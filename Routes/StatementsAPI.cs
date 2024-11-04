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

            //groupBuilder.MapGet("/", ([FromHeader(Name = AuthConfig.AppKeyHeaderName)] string hAppKey,
            //    [FromHeader(Name = AuthConfig.ApiKeyHeaderName)] string hApiKey) => Get(hAppKey,null))
            //    .RequireCors("AllowedOrigins");

            groupBuilder.MapGet("/", ([FromHeader(Name = AuthConfig.AppKeyHeaderName)] string hAppKey,
                [FromQuery(Name = "id")] int? id,
                [FromQuery(Name = "getall")] bool? getAllStatements) => Get(hAppKey,id,getAllStatements))
                .RequireCors("AllowedOrigins")
                .RequireAuthorization(); 
        }



        protected virtual async Task<ExpenseDTO> Get(string email, int? id, bool? getAllStatements)
        {
            StatementRequest request = new();
            CancellationToken cancellationToken = CancellationToken.None;
            request.getAllStatements = getAllStatements;
            if (id == null)
                request.Email = email;
            else
                request.CardId = id;
            ExpenseDTO? db_result = await _repositoryManager.statementService.GetStatementsAsync(request, cancellationToken);
            return db_result;
        }


        
    }
}
