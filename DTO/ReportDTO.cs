using IMC_CC_App.Models;
using Microsoft.Identity.Client;

namespace IMC_CC_App.DTO
{
    public class ReportDTO
    {
        public ReportDTO()
        {
            Status = new();
            Reports = [];
        }

        public CommonDTO Status { get; set; }
        public List<ReportDetail>? Reports { get; set; }
    }

    public class ReportDetail
    {
        public required int Id { get; set; }

        public required string Name { get; set; }

        public required string Status { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Modified { get; set; }

        public string? Memo { get; set; }
    }

    public class ReportDeleteRequest
    {
        public required int ReportId { get; set; }

        public required int[] ItemsToDelete { get; set; }
    }

    public class ReportNewRequest
    {
        public required string Name { get; set; }

        public required int CardNumber { get; set; }

        public string? Memo { get; set; }

        public StatusCategory Status { get; set; } = StatusCategory.PENDING;
    }

    public class ReportUpdateResponse
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required StatusCategory Status { get; set; }
        public string? Memo { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public DateTimeOffset? Created { get; set; }
        public int? CardNumber { get; set; }
    }

    public class ReportRequest
    {
        public required int ReportId { get; set; }

        public required StatusCategory Status { get; set; }

        public string? Memo { get; set; }
    }
}