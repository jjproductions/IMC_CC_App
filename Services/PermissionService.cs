using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Models;
using Microsoft.EntityFrameworkCore;
using System.Security;
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

        public async Task<List<AuthorizedUsers>> GetAuthorizedUsersAsync(string? id, bool getAllUsers = false)
        {
            List<AuthorizedUsers> response = new();

            if (_authorizedUsers == null)  //Get data for all users
                response = await GetAuthUserAsync(id, true);
            else if (!string.IsNullOrEmpty(id))  //Only make the call if user is not in cache
                if (!_authorizedUsers.ContainsKey(id)) 
                    response = await GetAuthUserAsync(id,getAllUsers);
                else response.Add(_authorizedUsers[id]);

            return response;
        }

        private async Task<List<AuthorizedUsers>> GetAuthUserAsync(string? id, bool getAllUsers = false)
        {
            List<AuthorizedUsers> response = new();
            List<AuthorizedUsersDB>? result = null;

            //Call DB Function
            if (!string.IsNullOrEmpty(id))
                result = await _context.GetAuthUserInfo(id, getAllUsers).ConfigureAwait(false);
            else return response;


            if (result != null && result.Count > 0)
            {//Found the user(s) and adding to cache
                AuthorizedUsers? aTemp = null;
                if (_authorizedUsers == null)
                    _authorizedUsers = [];
                //TODO: This should only be one item!!!
                foreach (AuthorizedUsersDB auDb in result)
                {
                    aTemp = new()
                    {
                        active = auDb.active,
                        emailAddress = auDb.email,
                        role = auDb.role_name,
                        roleId = auDb.role_id,
                        userName = auDb.name
                    };
                    
                    if (aTemp.emailAddress == id)
                        response.Add(aTemp);
                    
                    if(!_authorizedUsers.ContainsKey(aTemp.emailAddress))
                        _authorizedUsers.Add(aTemp.emailAddress, aTemp);
                    else
                    {
                        //TODO: log that there may be a dup in the DB & cache
                    }
                }
                return response;
            }

            //this means you're not in the list and i'm returning an empty response
            return response;
        }
    }
}
