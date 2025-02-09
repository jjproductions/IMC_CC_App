using IMC_CC_App.DTO;

namespace IMC_CC_App.Interfaces
{
    public interface IReport
    {
        Task<ReportDTO> GetOpenReports(int cardNumber, StatementStatus status = StatementStatus.OPEN);

        Task<Boolean> DeleteReport(int reportId, int[] itemsToDelete);

        Task<int> CreateReport(ReportNewRequest request);
    }
}