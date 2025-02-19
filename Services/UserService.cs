using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Models;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Services
{
    public class UserService : IUser
    {
        private readonly DbContext_CC _context;
        private readonly ILogger _logger;

        public UserService(DbContext_CC context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserDTO> CreateUserAsync(User userDTO, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO> GetAuthUserAsync(string? email = null)
        {
            UserDTO response = new();
            List<AuthorizedUsersDB>? userResponse = email != null ?
                await _context.GetAuthUserInfo(email).ConfigureAwait(false) :
                await _context.GetAuthUserInfo().ConfigureAwait(false);

            _logger.Warning($"GetAuthUserAsync returned: card - {userResponse[0].card_number} :: email - {userResponse[0].email}");

            foreach (AuthorizedUsersDB user in userResponse)
            {
                User? tempUser = new User
                {
                    Active = true,
                    Email = user.email,
                    Name = user.name,
                    Card = user.card_number,
                    CardId = null,
                    RoleName = user.role_name,
                    RoleId = user.role_id,
                    Id = user.id
                };
                response.Users?.Add(tempUser);
            }
            response.Status.StatusCode = 200;
            response.Status.Count = response.Users.Count;

            return response;
        }

        public async Task<UserDTO> GetUserAsync(string? email = null)
        {
            UserDTO response = new();

            List<UserDataDB>? results = email != null ?
                await _context.GetUserInfo(email).ConfigureAwait(false) :
                await _context.GetUserInfo().ConfigureAwait(false);

            foreach (UserDataDB user in results)
            {
                User? tempUser = new()
                {
                    Active = user.active,
                    Email = user.email,
                    Name = user.name,
                    Card = user.card_number,
                    CardId = user.card_id,
                    RoleName = user.role_name,
                    RoleId = user.role_id,
                    Id = user.id
                };
                response.Users?.Add(tempUser);
            }
            response.Status.StatusCode = 200;
            response.Status.Count = response.Users.Count;

            return response;
        }
    }
}
