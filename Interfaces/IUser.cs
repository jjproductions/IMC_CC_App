using IMC_CC_App.DTO;

namespace IMC_CC_App.Interfaces
{
    public interface IUser
    {
        Task<UserDTO> GetUserAsync(int id = 0);

        Task<UserDTO> CreateUserAsync(User userDTO, CancellationToken cancellationToken);
    }
}
