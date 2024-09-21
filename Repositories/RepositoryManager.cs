using IMC_CC_App.Data;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Services;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly DbContext_CC _context;
        private readonly ILogger _logger;
        private IExpense _expenseService;
        private IStatement _statementService;
        private IUser _userService;

        public RepositoryManager(DbContext_CC context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public IStatement statementService
        {
            get
            {
                if ( _statementService == null )
                    _statementService = new StatementService( _context, _logger);
                return _statementService;
            }
        }

        public IExpense expenseService
        {
            get
            {
                if (_expenseService == null)
                    _expenseService = new ExpenseService( _context, _logger);
                return _expenseService;
            }
        }

        public IUser userService
        {
            get
            {
                if ( _userService == null )
                    _userService = new UserService(_context, _logger);
                return _userService;

            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
