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

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/reports")
            .WithApiVersionSet(apiVersionSet)
            .RequireCors("AllowedOrigins")
                .RequireAuthorization();

            groupBuilder.MapGet("/", (int id, ClaimsPrincipal principal) => GetReports(id, principal));


            groupBuilder.MapPost("/delete", (
                [FromBody] ReportDeleteRequest request, ClaimsPrincipal principal) => DeleteReport(request.ReportId, request.ItemsToDelete, principal));

            groupBuilder.MapPost("/", (
                [FromBody] ReportNewRequest request, ClaimsPrincipal principal) => NewReport(request, principal));


        }

        protected virtual async Task<ReportDTO> GetReports(int id, ClaimsPrincipal principal)
        {
            _logger.Warning($"GetReports for {id}");
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            ReportDTO response = await _repositoryManager.reportService.GetOpenReports(id);
            _logger.Warning($@" 
                {(response?.Reports == null ?
                    $"returning GetReports: Status: {response?.Reports?[0].Status}" :
                    $"No Reports for report ID: {id}")}
            ");

            return response;
        }

        protected virtual async Task<Boolean> DeleteReport(int id, int[] itemsToDelete, ClaimsPrincipal principal)
        {
            _logger.Warning($"DeleteReport for {id}");
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            bool response = await _repositoryManager.reportService.DeleteReport(id, itemsToDelete);
            _logger.Warning($@" 
                {(response == true ?
                    $"successfully delete Report: {response}" :
                    $"Failed to delete report for report ID: {id}")}
            ");

            return response;
        }

        protected virtual async Task<int> NewReport(ReportNewRequest request, ClaimsPrincipal principal)
        {
            _logger.Warning($"NewReport for {request.Name}");
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            int response = await _repositoryManager.reportService.CreateReport(request);
            _logger.Warning($@" 
                {(response != 0 ?
                    $"successfully created Report: {response}" :
                    $"Failed to create report for report ID: {request.Name}")}
            ");

            return response;
        }

    }
}