
//using Microsoft.AspNetCore.Authorization;
//using ILogger = Serilog.ILogger;

//namespace IMC_CC_App.Security
//{
//    public class RoleAuthHandler : AuthorizationHandler<RolePolicyRequirement>
//    {
//        private readonly ILogger _iLogger;

//        public RoleAuthHandler(ILogger ILogger)
//        {
//            _iLogger = ILogger;
//        }
//        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolePolicyRequirement requirement)
//        {
//            string[]? allowAccess = context.Resource as string[];

//            if (allowAccess != null)
//            {
//                _iLogger.Warning($"RoleAuthHandler: {allowAccess.Length} role requirements.  {allowAccess[0]}");
//                context.Succeed(requirement);
//            }
//            return Task.CompletedTask;
//        }
//    }

//    public class RolePolicyRequirement : IAuthorizationRequirement
//    {

//    }
//}
