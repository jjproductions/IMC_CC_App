using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Models;
using IMC_CC_App.Utility;
using Microsoft.AspNetCore.Http.Timeouts;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Services
{
    public class ReportService : IReport
    {
        private readonly DbContext_CC _context;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public ReportService(DbContext_CC context, ILogger logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
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
            try
            {

                string statusString = request.Status.ToString();
                UpdateReport_SP response = await _context.UpdateReport(request.ReportId, request.Memo, statusString);
                if ((response?.name != null) && (response?.name != "") && (response?.cardid != null))
                {
                    // Send notification to staff
                    List<int> cardIds = response.cardid.HasValue ? new List<int> { response.cardid.Value } : new List<int>(); // Review this ***
                    NotiificationUtilityRequest utilityRequest = new NotiificationUtilityRequest
                    {
                        cardIds = cardIds,
                        sendToAdmin = false,
                        status = request.Status
                    };
                    var temp = await new Notifications(_context, _logger, _configuration).SendNotificationAsync(utilityRequest);
                    if (!temp)
                        _logger.Error("Successfully updated report but notifications failed");
                }
                return new ReportUpdateResponse
                {
                    Id = response != null ? response.id : 0,
                    Name = response != null ? response.name : "",
                    Status = response != null ? (StatusCategory)Enum.Parse(typeof(StatusCategory), response.status, true) : request.Status,
                    Memo = response != null ? response.memo : request.Memo
                };
            }
            catch (Exception ex)
            {
                _logger.Error($"ReportService-UpdateReportStatements: {ex.Message}");
                throw new Exception($"ReportService-UpdateReportStatements: {ex.Message}");
            }
        }

        public async Task<List<ReportUpdateResponse>> GetAdminReports()
        {
            List<ReportUpdateResponse> response = [];
            var sp_response = await _context.GetReports().ConfigureAwait(false);
            if (sp_response?.Count > 0)
            {
                foreach (var item in sp_response)
                {
                    response.Add(new ReportUpdateResponse
                    {
                        Id = item.id,
                        Name = item.name,
                        Status = (StatusCategory)Enum.Parse(typeof(StatusCategory), item.status, true),
                        Memo = item.memo,
                        Created = item.created,
                        Modified = item.modified,
                        CardNumber = item.card_number
                    });
                }
            }
            return response;
        }
    }
}