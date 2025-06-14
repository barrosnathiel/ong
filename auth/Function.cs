using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using ong_project.Appllication.Requests;
using ong_project.Dependency.LambdaDependency;
using ong_project.Helper;
using ong_project.Token;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace auth;

public class Function
{
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly IConfiguration _config;
    
    public Function()
    {
        var config = Configuration.GetConfiguration();

        _tokenGenerator = new JwtTokenGenerator(
            config["JWT_SECRET"] ?? throw new ArgumentNullException("JWT_SECRET is missing")
        );

        DependencyInjection.Inject(config); // << Passa a mesma config
    }
    
    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var login = JsonSerializer.Deserialize<UserLoginRequest>(request.Body);

        if (login.Username == "admin" && login.Password == "1234")
        {
            var token = _tokenGenerator.GenerateToken(login.Username);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(new { token }),
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };
        }

        return new APIGatewayProxyResponse
        {
            StatusCode = 401,
            Body = JsonSerializer.Serialize(new { error = "Unauthorized" }),
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        };
    }
}