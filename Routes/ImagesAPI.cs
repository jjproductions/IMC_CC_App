using System.Security.Claims;
using Asp.Versioning.Builder;
using IMC_CC_App.Components;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;


namespace IMC_CC_App.Routes
{
    public class ImagesAPI(IRepositoryManager repositoryManager, ILogger logger, IAuthorizationService authService) : RouterBase
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;
        private readonly ILogger _logger = logger;

        private readonly IAuthorizationService _authService = authService;

        public override void AddRoutes(WebApplication app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
               .HasApiVersion(new Asp.Versioning.ApiVersion(1))
               .ReportApiVersions()
               .Build();

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/images").WithApiVersionSet(apiVersionSet);

            groupBuilder.MapPost("/", (
                [FromQuery(Name = "rptId")] int rptId,
                [FromForm] IFormFile request,
                ClaimsPrincipal principal) => ImageUpload(rptId, request, principal))
                .RequireCors("AllowedOrigins")
                .RequireAuthorization()
                .DisableAntiforgery();
        }

        protected virtual async Task<string> ImageUpload(int rptId, IFormFile request, ClaimsPrincipal principal)
        {
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            return await _repositoryManager.imageService.UploadImages(rptId, request);
        }
    }
}