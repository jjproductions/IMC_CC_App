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
            UrlFragment = "expenses";
        }

        public override void AddRoutes(WebApplication app)
        {
            //app.MapGet($"/{UrlFragment}", () => Get());
            app.MapPost($"/{UrlFragment}", ([FromBody]List<ExpenseRequest> request) => Post(request));
        }

        protected virtual async Task<IResult> Post(List<ExpenseRequest> request)
        {
            var response = await _repositoryManager.expenseService.PostExpense(request);
            return Results.Ok();
        }

    }
}
