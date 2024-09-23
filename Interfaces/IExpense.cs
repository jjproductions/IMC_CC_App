using IMC_CC_App.DTO;

namespace IMC_CC_App.Interfaces
{
    public interface IExpense
    {

        Task<ExpenseDTO> GetExpensesAsync(int id = 0);

        Task<CommonDTO> PostExpenseAsync(List<ExpenseRequest> expense);
    }
}
