using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using IMC_CC_App.Interfaces;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Security
{
    public class AuthorizationMW
    {
        private readonly RequestDelegate _next;
        private ILogger _logger;
        private IRepositoryManager _repositoryManager;
       
        public AuthorizationMW(RequestDelegate next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            //APP key exists
            if (!context.Request.Headers.TryGetValue(AuthConfig.AppKeyHeaderName, out var extractedAppKey))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("AppKey header missing");
                return;
            }

            //API key exists
            if (!context.Request.Headers.TryGetValue(AuthConfig.ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("ApiKey header missing");
                return;
            }

            //Checksum validation on API key??

            Boolean isAuthUser = await ValidUser(extractedAppKey);

            if (!isAuthUser)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Not authorized to perform this operation");
                return;
            }
            
            await _next(context);
        }

        //TODO: return custom type that indicates statuscode
        //TODO: do i check role against endpoint?
        public async Task<Boolean> ValidUser(string? id)
        {
            Boolean isAuth = false;
            List<AuthorizedUsers> authUsers = await _repositoryManager.permissionService.GetAuthorizedUsersAsync(id);

            if (authUsers.Any())
            {
                isAuth = true;
            }

            return isAuth;
        }
    }
}
