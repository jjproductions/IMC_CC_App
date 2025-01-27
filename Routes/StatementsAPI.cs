using IMC_CC_App.Components;
using IMC_CC_App.Interfaces;
using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using IMC_CC_App.Security;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;
using ILogger = Serilog.ILogger;
using Microsoft.AspNetCore.Authorization;

namespace IMC_CC_App.Routes
{
    public class StatementsAPI(IRepositoryManager repositoryManager, ILogger logger, IAuthorizationService authService) : RouterBase
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

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/statements")
            .WithApiVersionSet(apiVersionSet)
            .RequireCors("AllowedOrigins");

            // return all statements for a user
            groupBuilder.MapGet("/", (
                [FromQuery(Name = "id")] int? id,
                [FromQuery(Name = "getall")] bool? getAllStatements,
                ClaimsPrincipal principal) => Get(principal, id, getAllStatements))
                .RequireAuthorization();

            // return all statements for a report
            groupBuilder.MapGet("/report", (
                [FromQuery(Name = "id")] int rptId,
                ClaimsPrincipal principal) => GetOpenStatements(rptId, principal))
                .RequireAuthorization();

            groupBuilder.MapPost("/", (
                [FromBody] StatementUpdateRequestDTO request,
                ClaimsPrincipal principal) => UpdateStatements(request, principal))
                .RequireAuthorization();
        }

        protected virtual async Task<ExpenseDTO> GetOpenStatements(int rptId, ClaimsPrincipal principal)
        {
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            ExpenseDTO response = new();
            Expense? expense = null;
            CancellationToken cancellationToken = CancellationToken.None;
            List<ReportStatments_SP>? sp_response = await _repositoryManager.statementService.GetReportStatements(rptId, cancellationToken);

            if (sp_response != null)
            {
                foreach (ReportStatments_SP rptItem in sp_response)
                {
                    expense = new()
                    {
                        Amount = rptItem.amount,
                        Category = rptItem.category,
                        Description = rptItem.description,
                        Type = rptItem.type,
                        Created = rptItem.created,
                        Id = rptItem.id,
                        Memo = rptItem.memo,
                        PostDate = rptItem.post_date.ToString(),
                        TransactionDate = rptItem.transaction_date.ToString("g"),
                        ReportID = rptItem.report_id
                    };
                    response.Expenses.Add(expense);
                }
            }

            return response;
        }

        protected virtual async Task<ExpenseDTO> Get(ClaimsPrincipal principal, int? id, bool? getAllStatements = false)
        {
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            _logger.Warning($"Get Statments - Auth claim: {principal.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value}...{authResult.Succeeded} :: id={id}");

            StatementRequest request = new();
            CancellationToken cancellationToken = CancellationToken.None;
            request.getAllStatements = getAllStatements;
            if (id == null)
                request.Email = principal.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            else
                request.CardId = id;
            ExpenseDTO? db_result = await _repositoryManager.statementService.GetStatementsAsync(request, cancellationToken);
            return db_result;
        }

        protected virtual async Task<int> UpdateStatements(StatementUpdateRequestDTO request, ClaimsPrincipal principal)
        {
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            _logger.Warning($"Post Statments - Auth claim: {principal.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value}...{authResult.Succeeded}");

            int response;
            response = await _repositoryManager.statementService.UpdateStatementsAsync(request, CancellationToken.None);
            return response;
        }

    }
}
