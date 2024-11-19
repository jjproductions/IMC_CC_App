using Asp.Versioning.Builder;
using IMC_CC_App.Components;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ILogger = Serilog.ILogger;
using IMC_CC_App.Utility;

namespace IMC_CC_App.Routes
{
    public class UserAPI(IRepositoryManager repositoryManager, ILogger logger, IAuthorizationService authService) : RouterBase
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

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/users")
                .WithApiVersionSet(apiVersionSet)
                .RequireCors("AllowedOrigins");

            groupBuilder.MapPost("/", ([FromBody] List<ExpenseRequest> request) => Post(request))
                .RequireAuthorization();

            groupBuilder.MapGet("/", (
                [FromQuery(Name = "allusers")] string? allUsers, ClaimsPrincipal principal) => Get(allUsers,principal))
                .RequireAuthorization();
        }

        protected virtual async Task<UserDTO> Get(string? allUsers, ClaimsPrincipal principal)
        {
            //Get Role info user info
            //TODO: if this fails??
            //var authResult = new ClaimsInfo();
            
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            _logger.Warning($"Get Users - Auth claim: {principal.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value}...{authResult.Succeeded}");
            if (string.IsNullOrEmpty(allUsers))
                return await _repositoryManager.userService.GetUserAsync(principal.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value);
            else
                return await _repositoryManager.userService.GetUserAsync();
        }

        private void Post(List<ExpenseRequest> request)
        {
            _logger.Warning("Received Expense Post");
        }
    }
}
