using IMC_CC_App.Components;
using IMC_CC_App.Interfaces;
using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using IMC_CC_App.Security;

namespace IMC_CC_App.Routes
{
    public class StatementsAPI : RouterBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public StatementsAPI(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            
        }

        public override void AddRoutes(WebApplication app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new Asp.Versioning.ApiVersion(1))
                .ReportApiVersions()
                .Build();

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/statements").WithApiVersionSet(apiVersionSet);

            groupBuilder.MapGet("/", ([FromHeader(Name = AuthConfig.AppKeyHeaderName)] string hApiKey,
                [FromHeader(Name = AuthConfig.ApiKeyHeaderName)] string hAppKey) => Get())
                .RequireCors("AllowedOrigins");
            
        }



        protected virtual async Task<ExpenseDTO> Get()
        {
            StatementRequest request = new();
            CancellationToken cancellationToken = CancellationToken.None;
            ExpenseDTO db_result = await _repositoryManager.statementService.GetStatementsAsync(request,cancellationToken);

            return db_result;
        }


        
    }
}
