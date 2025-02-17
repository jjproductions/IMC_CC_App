﻿using System.Security.Cryptography.X509Certificates;

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
        public int? CardId { get; set; }
        public required string RoleName { get; set; }
        public int? RoleId { get; set; }
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

    public class Login
    {
        public required string email { get; set; }
        public required string password { get; set; }
    }
}
