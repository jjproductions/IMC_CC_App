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
    public class ReportsAPI(IRepositoryManager repositoryManager, ILogger logger, IAuthorizationService authService) : RouterBase
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

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/reports").WithApiVersionSet(apiVersionSet);

            groupBuilder.MapGet("/", (int rptId, ClaimsPrincipal principal) => GetReports(rptId, principal))
                .RequireCors("AllowedOrigins")
                .RequireAuthorization();
        }

        protected virtual async Task<ReportDTO> GetReports(int rptId, ClaimsPrincipal principal)
        {
            _logger.Warning($"GetReports for {rptId}");
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            ReportDTO? response = await _repositoryManager.reportService.GetOpenReports(rptId);
            _logger.Warning($@" 
                {(response?.Reports == null ?
                    $"returning GetReports: Status: {response?.Reports?[0].Status}" :
                    $"No Reports for report ID: {rptId}")}
            ");

            return response;
        }

    }
}