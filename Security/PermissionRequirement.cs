using Microsoft.AspNetCore.Authorization;

namespace IMC_CC_App.Security;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        Permission = permission;   
    }
    public string Permission {get; }
}