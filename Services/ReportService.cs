using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Models;
using Microsoft.AspNetCore.Http.Timeouts;
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
                //_logger.Warning($"STATUS: {sp_response[i].status}");
                if (Enum.TryParse<StatusCategory>(sp_response[i].status, true, out StatusCategory result))
                {

                    reportDetail = new ReportDetail
                    {
                        Id = sp_response[i].id,
                        Name = sp_response[i].name,
                        Status = result.ToString(),
                        Created = sp_response[i].created,
                        Modified = sp_response[i].modified,
                        Memo = sp_response[i].memo
                    };
                    //_logger.Warning($"assigning Report Detail {reportDetail.Status}");
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

        public async Task<Boolean> DeleteReport(int reportId, int[] itemsToDelete)
        {
            return await _context.DeleteReport(reportId, itemsToDelete);
        }

        public async Task<int> CreateReport(ReportNewRequest request)
        {
            return await _context.CreateReport(request);
        }

        public async Task<ReportUpdateResponse> UpdateReportStatements(ReportRequest request)
        {
            string statusString = request.Status.ToString();
            UpdateReport_SP response = await _context.UpdateReport(request.ReportId, request.Memo, statusString);
            return new ReportUpdateResponse
            {
                Id = response.id,
                Name = response.name,
                Status = (StatusCategory)Enum.Parse(typeof(StatusCategory), response.status, true),
                Memo = response.memo
            };
        }
    }
}
