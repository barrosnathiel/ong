using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ong_project.Dependency.LambdaDependency;
using ong_project.Domain.Repositories;
using ong_project.Helper;
using ong_project.Mapper;
using ong_project.Token;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace course_list;

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
        // if (!request.Headers.TryGetValue("Authorization", out var authHeader) || !authHeader.StartsWith("Bearer "))
        // {
        //     return ErrorResponse(401, "Unauthorized");
        // }
        //
        // var token = authHeader.Substring("Bearer ".Length);
        // var principal = _tokenGenerator.ValidateToken(token);
        //
        // if (principal == null)
        // {
        //     return ErrorResponse(401, "Token inválido ou expirado.");
        // }
        
        var courseRepository = _serviceProvider.GetRequiredService<ICourseRepository>();

        var courses = await courseRepository.GetAllCourses();
        
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new  { result = Mapper.MapToDto(courses) }),
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        };
    }
}