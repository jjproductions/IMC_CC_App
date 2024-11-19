using System.Security.Claims;
using IMC_CC_App.DTO;
using Microsoft.AspNetCore.Authorization;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Utility
{
    public static class Status
    {
        public static CommonDTO SetStatus(int count, int code, string message)
        {
            CommonDTO response = new();
            if (code > 0 && !string.IsNullOrEmpty(message))
            {
                response.StatusMessage = message;
                response.StatusCode = code;
                response.Count = count;
            }

            return response;
        }
    }

    // public class ClaimsInfo
    // {
    //     private readonly IAuthorizationService _authService;
    //     private readonly ILogger _logger;

    //     public ClaimsInfo (IAuthorizationService authService, ILogger logger)
    //     {
    //         _authService = authService;
    //         _logger = logger;
    //     }
    //     public async Task<AuthorizationResult?> Authuser(ClaimsPrincipal cp, string policy)
    //     {
    //         AuthorizationResult? response = null;

    //         response = await _authService.AuthorizeAsync(cp, policy);
    //         _logger.Warning($"Get Users - Auth claim with policy {policy} is {cp.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value}...{response.Succeeded}");
    //         return response;
    //     }
    // }
}
