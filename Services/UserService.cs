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

        public Task<UserDTO> CreateUserAsync(User userDTO, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO> GetUserAsync(string? email = null)
        {
            UserDTO response = new();
            User? tempUser = null;
            List<AuthorizedUsersDB> results = await _context.GetAuthUserInfo(email);

            results = email != null ?
                await _context.GetAuthUserInfo(email).ConfigureAwait(false) :
                await _context.GetAuthUserInfo(email, true).ConfigureAwait(false);

            foreach (AuthorizedUsersDB user in results)
            {
                tempUser = new User
                {
                    Active = user.active,
                    Email = user.email,
                    Name = user.name
                };

                response.Users.Add(tempUser);
            }

            response.Status.StatusCode = 200;
            response.Status.Count = response.Users.Count;

            return response;
        }
    }
}
