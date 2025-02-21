using Asp.Versioning.Builder;
using IMC_CC_App.Components;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMC_CC_App.Security;
using ILogger = Serilog.ILogger;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IMC_CC_App.Routes
{
    public class ExpenseAPI(IRepositoryManager repositoryManager, ILogger logger, IAuthorizationService authService) : RouterBase
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

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/expenses").WithApiVersionSet(apiVersionSet);

            groupBuilder.MapPost("/", ([FromBody] List<ExpenseRequest> request, ClaimsPrincipal principal) => Post(request, principal))
                .RequireCors("AllowedOrigins")
                .RequireAuthorization();
        }

        protected virtual async Task<IResult> Post(List<ExpenseRequest> request, ClaimsPrincipal principal)
        {
            var authResult = await _authService.AuthorizeAsync(principal, "Admin");
            var response = await _repositoryManager.expenseService.PostExpenseAsync(request);
            return response.StatusCode == 200 ? Results.Ok() : Results.Problem(response.StatusMessage);
        }

    }
}
