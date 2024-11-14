using Asp.Versioning.Builder;
using Azure;
using IMC_CC_App.Components;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Security;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Routes
{

    public class SigninAPI : RouterBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public SigninAPI(IRepositoryManager repositoryManager, ILogger logger, IConfiguration config)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _config = config;
        }

        public override void AddRoutes(WebApplication app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
               .HasApiVersion(new Asp.Versioning.ApiVersion(1))
               .ReportApiVersions()
               .Build();

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/signin").WithApiVersionSet(apiVersionSet);

            groupBuilder.MapPost("/", ([FromBody] Login request) => Post(request))
                .RequireCors("AllowedOrigins")
                .AllowAnonymous();

        }

        protected virtual async Task<rType> Post(Login request)
        {
            rType response = new();
            //TODO: Use Redis
            var userInfo = await _repositoryManager.userService.GetUserAsync(request.email);
            if (userInfo?.Users.Count == 0)
                throw new UnauthorizedAccessException("Invalid Credentials");
            else
                response.access_token = TokenGenerator.GenerateToken(userInfo.Users?[0], _config.GetSection("AuthKey")?.Value?.ToString());

            return response;
            // return (new { Token = response });
            
                    
            
        }

        public class rType
        {
            public string access_token {get; set;}
        }
    }
}