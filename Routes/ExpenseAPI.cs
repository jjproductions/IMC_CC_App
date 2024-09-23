using Asp.Versioning.Builder;
using IMC_CC_App.Components;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IMC_CC_App.Routes
{
    public class ExpenseAPI : RouterBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public ExpenseAPI(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            
        }

        public override void AddRoutes(WebApplication app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
               .HasApiVersion(new Asp.Versioning.ApiVersion(1))
               .ReportApiVersions()
               .Build();

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/expenses").WithApiVersionSet(apiVersionSet);

            groupBuilder.MapPost("/", ([FromBody] List<ExpenseRequest> request) => Post(request))
                .RequireCors("AllowedOrigins");
        }

        protected virtual async Task<IResult> Post(List<ExpenseRequest> request)
        {
            var response = await _repositoryManager.expenseService.PostExpenseAsync(request);
            return Results.Ok();
        }

    }
}
