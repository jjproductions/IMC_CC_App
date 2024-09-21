using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMC_CC_App.Models
{
    [Table("usersAuth")]
    public class UserAuth
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        //[Required]
        //public Users User { get; set; }
    }
}
