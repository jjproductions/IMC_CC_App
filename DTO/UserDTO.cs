using System.Security.Cryptography.X509Certificates;
using IMC_CC_App.DTO;

namespace IMC_CC_App.DTO
{
    public class UserDTO
    {
        public UserDTO()
        {
            Status = new();
            Users = new();
        }

        public CommonDTO Status { get; set; }
        public List<User>? Users { get; set; } 
    }


    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public int? Card { get; set; }
        public Boolean Active { get; set; }
    }

    public class AuthorizedUsers
    {
        public required string userName { get; set; }

        public required string emailAddress { get; set; }

        public required bool active { get; set; }

        public required string role { get; set; }

        public required int roleId { get; set; }
    }
}
