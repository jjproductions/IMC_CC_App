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
            
            _iLogger.Warning("PermissionAuthHandler: role claim: " + role);

            if (role == requirement.Permission)
                context.Suceed(requirement);
            
            // if (context.User.GetPermissions().Contains(requirement.permission))
            //     context.Suceed(requirement);

            return;
        }
        
    }


}