using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Models;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Services
{
    public class ReportService : IReport
    {
        private readonly DbContext_CC _context;
        private readonly ILogger _logger;

        public ReportService(DbContext_CC context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReportDTO> GetOpenReports(int cardNumber, StatementStatus status = StatementStatus.OPEN)
        {
            _logger.Warning("entering GetOpenReports");
            ReportDetail? reportDetail = null;
            List<ReportDetail> reportList = [];
            ReportDTO response = new();
            var sp_response = await _context.GetReports_NotSubmitted(cardNumber).ConfigureAwait(false);

            _logger.Warning($"GetReports_NoSubmitted returned: {sp_response?.Count}");

            for (int i = 0; i < sp_response?.Count; i++)
            {
                _logger.Warning($"STATUS: {sp_response[i].status}");
                if (Enum.TryParse<StatusCategory>(sp_response[i].status, true, out StatusCategory result))
                {

                    reportDetail = new ReportDetail
                    {
                        Id = sp_response[i].id,
                        Name = sp_response[i].name,
                        Status = result.ToString(),
                        Created = sp_response[i].created,
                        Modified = sp_response[i].modified
                    };
                    _logger.Warning($"assigning Report Detail {reportDetail.Status}");
                    reportList.Add(reportDetail);
                }
                else
                {
                    _logger.Error($"ReportService-GetOpenReports: Status: {sp_response[i].status} for {sp_response[i].name} report is failing serialization");
                }
            }

            response.Reports = reportList;
            response.Status.StatusCode = 200;
            response.Status.Count = reportList.Count;
            response.Status.StatusMessage = "Success";
            return response;
        }


    }
}
