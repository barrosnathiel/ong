using System.Security.Claims;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ong_project.Application.Requests;
using ong_project.Dependency.LambdaDependency;
using ong_project.Domain.Repositories;
using ong_project.Helper;
using ong_project.Token;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace user_to_course;

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
        
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return ErrorResponse(401, "Token inválido ou expirado.");
        }
        
        var requestBody = JsonSerializer.Deserialize<UserToCourseRequest>(request.Body);
        
        if (requestBody is null || string.IsNullOrWhiteSpace(requestBody.CourseId))
        {
            return ErrorResponse(400, "Fields Must be provided.");
        }
        
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        
        var user = await userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return ErrorResponse(400, "User not found.");
        }
        
        var courseRepository = _serviceProvider.GetRequiredService<ICourseRepository>();
        
        var course = await courseRepository.GetCourseByIdAsync(requestBody.CourseId);
        if (course == null)
        {
            return ErrorResponse(400, "Course not found.");
        }

        if (course.UserId == user.Id)
        {
            return ErrorResponse(400, "Student is already in a course.");
        }
        
        await courseRepository.AddUserToCourseAsync(course.Id, user.Id);
        
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new { response = "Success" }),
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