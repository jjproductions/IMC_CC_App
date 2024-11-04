using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Services
{
    public class PermissionService : Interfaces.IPermission
    {
        private readonly DbContext_CC _context;
        private readonly ILogger _logger;
        private static Dictionary<string, AuthorizedUsers>? _authorizedUsers;
        
        public PermissionService(DbContext_CC context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string?> GetPermissions(string? email=null)
        {
            List<UserDataDB>? userInfo = email != null ?
                await _context.GetUserInfo(email).ConfigureAwait(false) : 
                null;

            if (userInfo != null && userInfo.Count == 1)
                return userInfo[0].role_name;
            
            return null;
        }


    }
}
