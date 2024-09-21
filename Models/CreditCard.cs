using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMC_CC_App.Models
{
    [Table("credit_card")]
    public class CreditCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CardNumber { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        public bool Active { get; set; } = true;


        
    }
}
