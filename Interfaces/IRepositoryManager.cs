namespace IMC_CC_App.Interfaces
{
    public interface IRepositoryManager
    {
        IStatement statementService { get; }
        IExpense expenseService { get; }
        
        IUser userService { get; }

        void Save();
    }
}
