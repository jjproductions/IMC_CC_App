using IMC_CC_App.Components;
using IMC_CC_App.Interfaces;
using IMC_CC_App.DTO;
using IMC_CC_App.Models;

namespace IMC_CC_App.Routes
{
    public class StatementsAPI : RouterBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public StatementsAPI(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            UrlFragment = "statements";
        }

        public override void AddRoutes(WebApplication app)
        {
            app.MapGet($"/{UrlFragment}", () => Get());
            //app.MapPost($"/{UrlFragment}", () => Post());
        }



        protected virtual async Task<ExpenseDTO> Get()
        {
            StatementRequest request = new();
            CancellationToken cancellationToken = CancellationToken.None;
            ExpenseDTO db_result = await _repositoryManager.statementService.GetStatements(request,cancellationToken);

            return db_result;
        }


        
    }
}
