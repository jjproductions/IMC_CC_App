using IMC_CC_App.DTO;

namespace IMC_CC_App.Interfaces
{
    public interface IExpense
    {

        Task<ExpenseDTO> GetExpenses(int id = 0);

        Task<CommonDTO> PostExpense(List<ExpenseRequest> expense);
    }
}
