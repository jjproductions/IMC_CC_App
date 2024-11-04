using Asp.Versioning.Builder;
using IMC_CC_App.Components;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Security;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Routes
{
    public class UserAPI : RouterBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger _logger;

        public UserAPI(IRepositoryManager repositoryManager, ILogger logger)
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

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/users").WithApiVersionSet(apiVersionSet);

            groupBuilder.MapPost("/", ([FromBody] List<ExpenseRequest> request) => Post(request))
                .RequireCors("AllowedOrigins")
                .RequireAuthorization();

            groupBuilder.MapGet("/", ([FromHeader(Name = AuthConfig.AppKeyHeaderName)] string hAppKey,
                [FromQuery(Name = "allusers")] string? allUsers) => Get(hAppKey, allUsers))
                .RequireCors("AllowedOrigins");
                // .RequireAuthorization();
        }

        protected virtual async Task<UserDTO> Get(string hAppKey, string? allUsers)
        {
            if (string.IsNullOrEmpty(allUsers))
                return await _repositoryManager.userService.GetUserAsync(hAppKey);
            else
                return await _repositoryManager.userService.GetUserAsync();
        }

        private void Post(List<ExpenseRequest> request)
        {
            _logger.Warning("Received Expense Post");
        }
    }
}
