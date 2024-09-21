using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMC_CC_App.Models
{
    [Table("type")]
    public class Type
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    }
}
