using System.Security.Claims;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ong_project.Common;
using ong_project.Dependency.LambdaDependency;
using ong_project.Domain.Repositories;
using ong_project.Helper;
using ong_project.Mapper;
using ong_project.Token;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace user_list;

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
            return ErrorResponse(401, "Token inválido ou expirado.");
        var token = authHeader["Bearer ".Length..];
        var principal = _tokenGenerator.ValidateToken(token);
        var role = principal?.FindFirst(ClaimTypes.Role)?.Value;

        if (principal == null)
        {
            return ErrorResponse(401, "Token inválido ou expirado.");
        }
        if (role != Constants.ADMIN_ROLE) return ErrorResponse(401, "Unauthorized");
        var courseRepository = _serviceProvider.GetRequiredService<IUserRepository>();

        var users = await courseRepository.GetAllUsersAsync();
            
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new { result = Mapper.MapToDTO(users) }),
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        };
    }
    
    private static APIGatewayProxyResponse ErrorResponse(int statusCode, string errorMessage)
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