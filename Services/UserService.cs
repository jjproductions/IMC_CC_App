using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
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

        public Task<UserDTO> GetUserAsync(int id = 0)
        {
            throw new NotImplementedException();
        }
    }
}
