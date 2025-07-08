using System.Security.Claims;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ong_project.Application.Requests;
using ong_project.Common;
using ong_project.Dependency.LambdaDependency;
using ong_project.Domain;
using ong_project.Domain.Repositories;
using ong_project.Helper;
using ong_project.Token;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace course_create;

public class Function
{
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;
    
    public Function()
    {
        _config = Configuration.GetConfiguration();
        _serviceProvider = DependencyInjection.Inject(_config);
        _tokenGenerator = new JwtTokenGenerator(_config["JWT_SECRET"] 
                                                ?? throw new ArgumentNullException("JWT_SECRET is missing"));
    }
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (request.Headers == null ||
            !request.Headers.TryGetValue("Authorization", out var authHeader) ||
            !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return ErrorResponse(401, "Unauthorized");
        var token = authHeader["Bearer ".Length..];
        var principal = _tokenGenerator.ValidateToken(token);
        if (principal == null)
        {
            return ErrorResponse(401, "Token inválido ou expirado.");
        }
            
        var role = principal?.FindFirst(ClaimTypes.Role)?.Value;

        if (role != Constants.ADMIN_ROLE) return ErrorResponse(401, "Unauthorized");
        var requestBody = JsonSerializer.Deserialize<CourseRequest>(request.Body);
            
        if (requestBody is null || string.IsNullOrWhiteSpace(requestBody.Name) ||
            requestBody.Begin == DateTime.MinValue || requestBody.End == DateTime.MinValue)
        {
            return ErrorResponse(400, "Email and password are required.");
        }
            
        var courseRepository = _serviceProvider.GetRequiredService<ICourseRepository>();

        await courseRepository.CreateClassAsync(new Course
        {
            Name = requestBody.Name,
            Begin = requestBody.Begin.EnsureUtc(),
            End = requestBody.End.EnsureUtc(),
            CreatedAt = DateTime.UtcNow.EnsureUtc(),
            TotalHours = requestBody.TotalHours,
            IsActive = true,
            Token = Guid.NewGuid().ToString()
        });
            
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new { result = "Course Created" }),
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        };

    }
    
    private APIGatewayProxyResponse ErrorResponse(int statusCode, string errorMessage)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
            Body = JsonSerializer.Serialize(new { error = errorMessage }),
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        };
    }
}