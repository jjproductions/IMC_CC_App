using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMC_CC_App.Models
{
    [Table("reports")]
    public class Report
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset Modified { get; set; }

        public required StatusCategory Status { get; set; } = StatusCategory.PENDING;

        public required int CardId { get; set; }
    }

    public class Report_SP
    {
        public int id { get; set; }

        public required string name { get; set; }

        public required string status { get; set; }

        public DateTimeOffset created { get; set; }

        public DateTimeOffset modified { get; set; }

        public string? memo { get; set; }
    }

    public enum StatusCategory
    {
        PENDING = 0,
        SUBMITTED = 1,
        APPROVED = 2,
        RETURNED = 3,
        DELETED = 4
    }

}