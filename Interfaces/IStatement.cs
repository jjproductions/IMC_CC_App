using IMC_CC_App.DTO;

namespace IMC_CC_App.Interfaces
{
    public interface IStatement
    {

        Task<ExpenseDTO> UploadStatements(string statement, CancellationToken cancellationToken);
        Task<ExpenseDTO> GetStatements(StatementRequest request, CancellationToken cancellationToken);

    }
}
