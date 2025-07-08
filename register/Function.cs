using System.Security.Claims;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ong_project.Application.Requests;
using ong_project.Appllication.Requests;
using ong_project.Common;
using ong_project.Dependency.LambdaDependency;
using ong_project.Domain;
using ong_project.Domain.Repositories;
using ong_project.Helper;
using ong_project.Token;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace register;

public class Function
{
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;
    private readonly JwtTokenGenerator _tokenGenerator;

    public Function()
    {
        _config = Configuration.GetConfiguration();
        _serviceProvider = DependencyInjection.Inject(_config);
        _tokenGenerator = new JwtTokenGenerator(_config["JWT_SECRET"]
                                                ?? throw new ArgumentNullException("JWT_SECRET is missing"));
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var user = new User();
        var isAdmin = false;

        var requestBody = JsonSerializer.Deserialize<RegisterRequest>(request.Body);

        if (requestBody is null || string.IsNullOrWhiteSpace(requestBody.Email) ||
            string.IsNullOrWhiteSpace(requestBody.Password))
        {
            return new APIGatewayProxyResponse 
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new { error = "Email and password are required." }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        if (!requestBody.Email.IsValidEmail())
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new { error = "Invalid email format." }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        var userRepo = _serviceProvider.GetRequiredService<IUserRepository>();

        var exist = await userRepo.GetByEmailAsync(requestBody.Email);
        if (exist != null)
            return new APIGatewayProxyResponse
            {
                StatusCode = 401,
                Body = JsonSerializer.Serialize(new { error = "User Already Exists." }),
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };


        if (request.Headers != null &&
            request.Headers.TryGetValue("Authorization", out var authHeader) &&
            authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader["Bearer ".Length..];
            var principal = _tokenGenerator.ValidateToken(token);
            var role = principal?.FindFirst(ClaimTypes.Role)?.Value;

            isAdmin = role == Constants.ADMIN_ROLE;
        }

        user.UserType = isAdmin
            ? requestBody.Type == "PRO" 
                ? Constants.USER_PROFESSOR
                : requestBody.Type == "ADMIN"
                    ? Constants.ADMIN_ROLE
                    : Constants.USER_ROLE
            : Constants.USER_ROLE;

        user.Email = requestBody.Email;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(requestBody.Password);
        user.Address = requestBody.Address;
        user.PhoneNumber = requestBody.PhoneNumber;
        user.Cpf = requestBody.CPF;
        user.Name = requestBody.Name;
        user.Token = Guid.NewGuid().ToString();

        await userRepo.CreateUserAsync(user);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new { sucesso = "User Created." }),
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        };
    }
}