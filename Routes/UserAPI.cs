using Asp.Versioning.Builder;
using IMC_CC_App.Components;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Security;
using Microsoft.AspNetCore.Mvc;

namespace IMC_CC_App.Routes
{
    public class UserAPI : RouterBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public UserAPI(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public override void AddRoutes(WebApplication app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
               .HasApiVersion(new Asp.Versioning.ApiVersion(1))
               .ReportApiVersions()
               .Build();

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/users").WithApiVersionSet(apiVersionSet);

            groupBuilder.MapPost("/", ([FromBody] List<ExpenseRequest> request) => Post(request))
                .RequireCors("AllowedOrigins");

            groupBuilder.MapGet("/", ([FromHeader(Name = AuthConfig.AppKeyHeaderName)] string hAppKey,
                [FromHeader(Name = AuthConfig.ApiKeyHeaderName)] string hApiKey,
                [FromQuery(Name = "allusers")] string? allUsers) => Get(hAppKey, allUsers))
                .RequireCors("AllowedOrigins");
        }

        protected virtual async Task<UserDTO> Get(string hAppKey, string? allUsers)
        {
            if (string.IsNullOrEmpty(allUsers))
                return await _repositoryManager.userService.GetUserAsync(hAppKey);
            else
                return await _repositoryManager.userService.GetUserAsync();
        }

        private object Post(List<ExpenseRequest> request)
        {
            throw new NotImplementedException();
        }
    }
}
