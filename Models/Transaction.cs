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

        public int CardId { get; set; }

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

    // stored procedure to get transactions
    public class StatmentsDB
    {
        public required int id { get; set; }

        [Required]
        public DateTimeOffset transaction_date { get; set; }

        [Required]
        public DateTimeOffset post_date { get; set; }

        public required string description { get; set; }

        public required int card_number { get; set; }

        [Required]
        public double amount { get; set; }

        public required string category { get; set; }

        public required string type { get; set; }

        [Required]
        public DateTimeOffset created { get; set; } = DateTimeOffset.UtcNow;

        public string? memo { get; set; }

        public int? report_id { get; set; }

        public string? receipt_url { get; set; }
    }

    // Stored procedure to get transactions for a report
    public class ReportStatments_SP
    {
        public required int id { get; set; }

        [Required]
        public DateTimeOffset transaction_date { get; set; }

        [Required]
        public DateTimeOffset post_date { get; set; }

        public required string description { get; set; }

        [Required]
        public double amount { get; set; }

        public required string category { get; set; }

        public required string type { get; set; }

        [Required]
        public DateTimeOffset created { get; set; } = DateTimeOffset.UtcNow;

        public string? memo { get; set; }

        public int? report_id { get; set; }

        public string? receipt_url { get; set; }
    }
}
