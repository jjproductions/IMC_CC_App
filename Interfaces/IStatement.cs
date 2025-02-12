using IMC_CC_App.DTO;
using IMC_CC_App.Models;

namespace IMC_CC_App.Interfaces
{
    public interface IStatement
    {

        Task<List<ReportStatments_SP>> GetReportStatements(int cardNumber, CancellationToken cancellationToken);
        Task<ExpenseDTO> GetStatementsAsync(StatementRequest request, CancellationToken cancellationToken);
        Task<int> UpdateStatementsAsync(StatementUpdateRequestDTO statements, CancellationToken cancellationToken);

        Task<ExpenseDTO> UpdateReportStatementsAsync(StatementUpdateRequestDTO statements, CancellationToken cancellationToken);

    }
}
