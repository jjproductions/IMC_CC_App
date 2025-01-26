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

        public string Memo { get; set; }
    }
}