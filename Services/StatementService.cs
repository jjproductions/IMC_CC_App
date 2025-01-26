using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using IMC_CC_App.Interfaces;
using ILogger = Serilog.ILogger;
using Microsoft.EntityFrameworkCore;
using Azure.Core;

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
            }
            else
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
                    PostDate = statement.post_date.ToString().Split(" ")[0],
                    TransactionDate = statement.transaction_date.ToString().Split(" ")[0],
                    ReportID = statement.report_id
                };

                response.Expenses.Add(expense);
            }

            if (response.Expenses.Count > 0)
            {
                response.Status.Count = response.Expenses.Count();
                response.Status.StatusCode = 200;
                return response;
            }

            return response;
        }

        public async Task<List<ReportStatments_SP>> GetReportStatements(int rptId, CancellationToken cancellationToken)
        {
            List<ReportStatments_SP>? sp_response = null;
            sp_response = await _context.GetReportStatements(rptId);

            return sp_response;
        }

        public async Task<ExpenseDTO> UpdateStatementsAsync(int rptId, List<StatementUpdateRequest> statements, CancellationToken cancellationToken)
        {
            ExpenseDTO response = new();
            bool result = false;
            int reportId = -1;
            if (rptId > 0) {
                reportId = rptId;
                result = true;
            }
            else
            {
                //create new Rpt, get new RptID and set it to reportId
            }

            if (result)
                result = await _context.UpdateStatements(reportId, statements);
            else
            {
                response.Status.StatusMessage = "Error creating new report";
                return response;
            }

            if (result)
            {
                response.Status.Count = statements.Count;
                response.Status.StatusCode = 200;
                response.Status.StatusMessage = "Statements updated successfully";
            }
            else
            {
                response.Status.StatusMessage = "Error updating statements";
            }

            return response;
        }
    }
}
