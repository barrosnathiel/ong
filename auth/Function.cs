using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ong_project.Application.Requests;
using ong_project.Appllication.Requests;
using ong_project.Dependency.LambdaDependency;
using ong_project.Domain.Repositories;
using ong_project.Helper;
using ong_project.Token;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace auth;

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
                                                ?? throw new ArgumentNullException("JWT_SECRET is missing") );
    }
    
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var requestBody = JsonSerializer.Deserialize<AuthRequest>(request.Body);
        
        if (requestBody is null || string.IsNullOrWhiteSpace(requestBody.Email) || string.IsNullOrWhiteSpace(requestBody.Password))
        {
            return ErrorResponse(400, "Email and password are required.");
        }

        if (!requestBody.Email.IsValidEmail())
        {
            return ErrorResponse(400, "Invalid email format.");
        }
        
        var userRepo = _serviceProvider.GetRequiredService<IUserRepository>();
        var user = await userRepo.GetByEmailAsync(requestBody.Email);

        if (user == null) return ErrorResponse(401, "Unauthorized");
        var isValidPassword = BCrypt.Net.BCrypt.Verify(requestBody.Password, user.PasswordHash);
        if (isValidPassword)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(new { Token = _tokenGenerator.GenerateToken(user.Email, user.UserType) }),
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };
        }

        return ErrorResponse(404, "Password is not valid");
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