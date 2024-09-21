using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using IMC_CC_App.Interfaces;
using ILogger = Serilog.ILogger;
using Microsoft.EntityFrameworkCore;

namespace IMC_CC_App.Services
{
    public class StatementService : IStatement
    {
        private readonly DbContext_CC _context;
        private readonly ILogger _logger;
        public StatementService(DbContext_CC context, ILogger logger) 
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ExpenseDTO> GetStatements(StatementRequest request, CancellationToken cancellationToken)
        {
            ExpenseDTO response = new ExpenseDTO();
            Expense transact = null;
            CreditCardDTO CardInfo = new();
            var query = (
                from t in _context.Transactions
                join c in _context.CreditCards on t.CardId equals c.Id
                join ct in _context.Categories on t.CategoryId equals ct.Id
                join tp in _context.Types on t.TypeId equals tp.Id
                select new Expense
                {
                    Id = t.Id,
                    TransactionDate = t.TransactionDate,
                    PostDate = t.PostDate,
                    Description = t.Description,
                    CardNumber = c.CardNumber,
                    Amount = t.Amount,
                    Category = ct.Name,
                    Type = tp.Name,
                    Created = t.Created,
                    Memo = t.Memo,
                    ReportID = t.Report_Id,
                    
                });

            var results = await query.ToListAsync().ConfigureAwait(false);
            if (results.Any())
            {
                response.Expenses = (List<Expense>)results;
                response.Status.Count = results.Count();
                return response;
            }


            return response;
        }

        public Task<ExpenseDTO> UploadStatements(string statement, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
