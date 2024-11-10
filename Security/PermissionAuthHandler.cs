using Microsoft.AspNetCore.Authorization;
using IMC_CC_App.Repositories;
using System.Security.Claims;
using IMC_CC_App.Models;
using IMC_CC_App.Interfaces;
using IMC_CC_App.DTO;
using ILogger = Serilog.ILogger; 

namespace IMC_CC_App.Security
{
    public class PermissionAuthHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger _iLogger;

        public PermissionAuthHandler(IRepositoryManager repositoryManager, ILogger iLogger)
        {
            _repositoryManager = repositoryManager;
            _iLogger = iLogger;
        }

        protected override async Task HandleRequirementAsync (AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            string? role = context.User?.Claims.SingleOrDefault(x => x.Type == CustomClaims.Role)?.Value;

            bool hasCustomRole = context.User?.HasClaim(x => x.Type == CustomClaims.Role) ?? false;
            
            bool hasEmail = context.User?.HasClaim(x => x.Type == ClaimTypes.Email) ?? false;

            bool hasRole = context.User?.HasClaim(x => x.Type == CustomClaims.Role) ?? false;

            _iLogger.Warning("PermissionAuthHandler: role claim: " + role);

            if (!string.IsNullOrEmpty(role) && role == requirement.Permission)
                context.Succeed(requirement);
            
            if (hasEmail)
                _iLogger.Warning($"Handler: Email is {context.User?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value}");

            if (hasRole)
                _iLogger.Warning($"Handler: Role is {context.User?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value}");

            if (hasCustomRole)
                _iLogger.Warning($"Handler: Custom Role is {context.User?.Claims.SingleOrDefault(x => x.Type == CustomClaims.Role)?.Value}");

        }
        
    }


}