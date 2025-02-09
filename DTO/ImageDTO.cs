using System.ComponentModel.DataAnnotations;

namespace IMC_CC_App.DTO
{
    public class ImageDTO
    {
        public ImageDTO()
        {
            Status = new();
            Images = new();
        }

        [Required]
        public CommonDTO Status { get; set; }
        public List<BlobData> Images { get; set; }
    }

    public class BlobData
    {
        public required string Name { get; set; }
        public required string Url { get; set; }
        public required string ContentType { get; set; }
        public required Stream Content { get; set; }
    }

    public class ImageUploadRequest
    {
        public required string Name { get; set; }
        public required IFormFile Image { get; set; }
        public required string ContentType { get; set; }
        public required int ExpenseId { get; set; }
        public required int ReportId { get; set; }

    }

    public class ImageUploadResponse
    {
        public required int ExpenseId { get; set; }
        public required string Url { get; set; }
    }

}