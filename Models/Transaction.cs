using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMC_CC_App.Models
{
    [Table("transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTimeOffset TransactionDate { get; set; }

        [Required]
        public DateTimeOffset PostDate { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public string Description { get; set; }

        public int? CardId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        public string? Memo { get; set; }

        public int? Report_Id { get; set; }

        public int CreatedBy { get; set; } = 0;

        //public string? UserName { get; set; } = null;

        //[Required]
        //public CreditCard CardInfo { get; set; }
    }
}
