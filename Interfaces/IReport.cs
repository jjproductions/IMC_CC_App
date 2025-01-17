using IMC_CC_App.DTO;

namespace IMC_CC_App.Interfaces
{
    public interface IReport
    {
        Task<ReportDTO> GetOpenReports(int cardNumber, StatementStatus status = StatementStatus.OPEN);
    }
}