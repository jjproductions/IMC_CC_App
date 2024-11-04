using Asp.Versioning.Builder;
using IMC_CC_App.Components;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Routes
{

    public class SigninAPI : RouterBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger _logger;

        public SigninAPI(IRepositoryManager repositoryManager, ILogger logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
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

        protected virtual rType Post(Login request)
        {
            rType response = new();
            response.access_token = TokenGenerator.GenerateToken(request.email);

            return response;
            // return (new { Token = response });
            
                 
            
        }

        public class rType
        {
            public string access_token {get; set;}
        }
    }
}