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
                    ReportID = statement.report_id,
                    ReceiptUrl = statement.receipt_url
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
            return await _context.GetReportStatements(rptId);
        }

        // public async Task<int> UpdateStatementsAsync(StatementUpdateRequestDTO statements, CancellationToken cancellationToken)
        // {
        //     bool result = false;
        //     int reportId = -1;
        //     int response;
        //     _logger.Warning($"StatementsService:UpdateStatementsAsync - report id: {statements.ReportId}");
        //     Console.WriteLine($"StatementsService:UpdateStatementsAsync - report id: {statements.ReportId}");
        //     if (statements.ReportId != null)
        //     {
        //         reportId = (int)statements.ReportId;
        //         result = true;
        //     }
        //     else
        //     {
        //         //create new Rpt, get new RptID and set it to reportId
        //         int newReportId = await _context.CreateReport(statements.CardNumber, statements.ReportName, statements.ReportMemo);
        //         if (newReportId > 0)
        //         {
        //             reportId = newReportId;
        //             result = true;
        //             response = newReportId;
        //         }
        //     }

        //     if (result)
        //         result = await _context.UpdateStatements(reportId, statements);

        //     response = result ? reportId : -1;

        //     return response;
        // }

        public async Task<ExpenseDTO> UpdateReportStatementsAsync(StatementUpdateRequestDTO statements, CancellationToken cancellationToken)
        {
            bool result = false;
            int reportId = -1;
            List<ReportStatments_SP> response_SP = [];
            ExpenseDTO response = new();
            Expense? expense = null;
            _logger.Warning($"StatementsService:UpdateReportStatementsAsync - report id: {statements.ReportId}");
            Console.WriteLine($"StatementsService:UpdateReportStatementsAsync - report id: {statements.ReportId}");
            try
            {
                // updating an existing report
                if (statements.ReportId != null)
                {
                    reportId = (int)statements.ReportId;
                    result = true;
                }
                // else
                // {
                //     //create new Rpt, get new RptID and set it to reportId
                //     ReportNewRequest request = new()
                //     {
                //         CardNumber = statements.CardNumber,
                //         Name = statements.ReportName,
                //         Memo = statements.ReportMemo,
                //         Status = statements.Status
                //     };

                //     int newReportId = await _context.CreateReport(request);
                //     if (newReportId > 0)
                //     {
                //         reportId = newReportId;
                //         result = true;
                //     }
                // }

                if (result)
                    response_SP = await _context.UpdateReportStatements(reportId, statements);

                if (response_SP.Count > 0)
                {
                    foreach (ReportStatments_SP rptItem in response_SP)
                    {
                        expense = new()
                        {
                            Amount = rptItem.amount,
                            Category = rptItem.category,
                            Description = rptItem.description,
                            Type = rptItem.type,
                            Created = rptItem.created,
                            Id = rptItem.id,
                            Memo = rptItem.memo,
                            PostDate = rptItem.post_date.ToString("g"),
                            TransactionDate = rptItem.transaction_date.ToString("g"),
                            ReportID = rptItem.report_id,
                            ReceiptUrl = rptItem.receipt_url
                        };
                        _logger.Warning($"UpdateReportStatementsAsync - Report {expense.ReportID} Expense ID: {expense.Id} :: Receipt URL: {expense.ReceiptUrl}");
                        response.Expenses.Add(expense);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateReportStatementsAsync: {ex.Message}");
                response.Expenses = [];
            }

            return response;
        }
    }
}
