using IMC_CC_App.DTO;

namespace IMC_CC_App.Interfaces
{
    public interface IStatement
    {

        Task<ExpenseDTO> UploadStatementsAsync(string statement, CancellationToken cancellationToken);
        Task<ExpenseDTO> GetStatementsAsync(StatementRequest request, CancellationToken cancellationToken);

    }
}
