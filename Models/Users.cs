using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMC_CC_App.Models
{
    [Table("users")]
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int CardId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public bool Active { get; set; } = true;
        public string? Comments { get; set; }

        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        //public CreditCard? CardInfo { get; set; }

        /*[Required]
        public Roles RoleInfo { get; set; }*/

    }

    
    public class AuthorizedUsersDB
    {
        public required string name { get; set; }

        public required string email { get; set; }

        public required bool active { get; set; }

        public required string role_name { get; set; }

        public required int role_id { get; set; }

        public required int id {get; set; }
    }

    public class UserDataDB
    {
        public required string name { get; set; }

        public required string email { get; set; }

        public required bool active { get; set; }

        public required string role_name { get; set; }

        public required int role_id { get; set; }

        public required int card_number {get; set;}

        public required int card_id {get; set;}

        public required int id {get; set; }
    }
}
