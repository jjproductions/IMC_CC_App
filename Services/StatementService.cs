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
        private ILogger _logger;
        
        public StatementService(DbContext_CC context, ILogger logger) 
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ExpenseDTO> GetStatementsAsync(StatementRequest request, CancellationToken cancellationToken)
        {
            ExpenseDTO response = new ExpenseDTO();
            List<StatmentsDB>? statementsResults = null;
            Expense? expense = null;

            //_logger.Warning("get all statements: " + request.getAllStatements);
            if (request.getAllStatements != null && request.getAllStatements == true)
            {
                statementsResults = await _context.GetStatements(request.CardId, true).ConfigureAwait(false);
                //_logger.Warning("called by card id");
            }else
                statementsResults = string.IsNullOrEmpty(request.Email)
                    ? await _context.GetStatements(request.CardId).ConfigureAwait(false)
                    : await _context.GetStatements(request.Email).ConfigureAwait(false);

            foreach (var statement in statementsResults)
            {
                expense = new()
                {
                    Amount = statement.amount,
                    Category = statement.category,
                    Description = statement.description,
                    Type = statement.type,
                    CardNumber = statement.card_number,
                    Created = statement.created,
                    Id = statement.id,
                    Memo = statement.memo,
                    PostDate = statement.post_date,
                    TransactionDate = statement.transaction_date,
                    ReportID = statement.report_id
                };

                response.Expenses.Add(expense);
            }

            if (response.Expenses.Count>0)
            {
                response.Status.Count = response.Expenses.Count();
                response.Status.StatusCode = 200;
                return response;
            }

            return response;
        }

        public Task<ExpenseDTO> UploadStatementsAsync(string statement, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
