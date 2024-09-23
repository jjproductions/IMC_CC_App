using IMC_CC_App.DTO;

namespace IMC_CC_App.Interfaces
{
    public interface IPermission
    {
        Task<List<AuthorizedUsers>> GetAuthorizedUsersAsync(string? id, bool getAllUsers=false);
    }
}
