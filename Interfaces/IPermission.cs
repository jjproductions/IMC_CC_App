using IMC_CC_App.DTO;

namespace IMC_CC_App.Interfaces
{
    public interface IPermission
    {
        Task<string?> GetPermissions(string? email=null);
    }
}
