using IMC_CC_App.Data;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Services;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly IConfiguration? _configuration;
        private readonly DbContext_CC _context;
        private readonly ImagesStoreContext _contextImages;
        private readonly ILogger _logger;
        private IExpense _expenseService;
        private IStatement _statementService;
        private IUser _userService;
        //private IPermission _permissionService;
        private IImage _imageService;

        private IReport _reportService;

        public RepositoryManager(DbContext_CC context, ILogger logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        //public IPermission permissionService
        //{
        //    get
        //    {
        //        if (_permissionService == null)
        //            _permissionService = new PermissionService(_context, _logger);
        //        return _permissionService;
        //    }
        //}

        public IStatement statementService
        {
            get
            {
                _statementService ??= new StatementService(_context, _logger);
                return _statementService;
            }
        }

        public IExpense expenseService
        {
            get
            {
                _expenseService ??= new ExpenseService(_context, _logger);
                return _expenseService;
            }
        }

        public IUser userService
        {
            get
            {
                _userService ??= new UserService(_context, _logger);
                return _userService;

            }
        }

        public IReport reportService
        {
            get
            {
                _reportService ??= new ReportService(_context, _logger);
                return _reportService;
            }
        }

        public IImage imageService
        {
            get
            {
#pragma warning disable CS8604 // Possible null reference argument.
                _imageService ??= new ImageService(_contextImages, _logger, _configuration);
#pragma warning restore CS8604 // Possible null reference argument.
                return _imageService;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
