using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Models;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using ILogger = Serilog.ILogger;
using IMC_CC_App.Utility;

namespace IMC_CC_App.Services
{
    public class ExpenseService : IExpense
    {
        private readonly ILogger _logger;
        private readonly DbContext_CC _context;
        private static Dictionary<string, int> _category;
        private static Dictionary<string, int> _type;

        public ExpenseService(DbContext_CC context, ILogger logger) 
        {
            _context = context;
            _logger = logger;
        }

        public Task<ExpenseDTO> GetExpensesAsync(int id = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<CommonDTO> PostExpenseAsync(List<ExpenseRequest> request)
        {
            _logger.Warning("entering ExpenseService");
            Transaction? tranItem = null;
            CommonDTO response = new();
            ConsolidatedInfo consolidatedInfo = await DataOb.GetConsolidatedInfo(_context);
            _logger.Warning("ExpenseService");
            Dictionary<int, string> errorCollection = new Dictionary<int, string>();
            string errormsg = "";
            int count = 1;
            int count_success = 0;
            
            _logger.Warning("start loop");
            foreach (var item in request)
            {

                errormsg = "";
                tranItem = new()
                {
                    Amount = item.Amount,
                    Description = item.Description,
                    Memo = item.Memo,
                    PostDate = item.PostDate.ToUniversalTime(),
                    TransactionDate = item.TransactionDate.ToUniversalTime(),
                };


                if (consolidatedInfo.creditCard.ContainsValue(item.CardNumber))
                    tranItem.CardId = consolidatedInfo.creditCard.First(t => t.Value == item.CardNumber).Key;
                else
                    errormsg = errormsg + " - card number: " + item.CardNumber;

                if (consolidatedInfo.type.ContainsValue(item.Type))
                {
                    tranItem.TypeId = consolidatedInfo.type.First(t=>t.Value == item.Type).Key; 
                }
                else
                    errormsg = errormsg + " - type: " + item.Type;

                if (consolidatedInfo.category.ContainsValue(item.Category))
                {
                    tranItem.CategoryId = consolidatedInfo.category.First(t => t.Value == item.Category).Key;
                }
                else
                    errormsg = errormsg + " - category: " + item.Category;

                if (string.IsNullOrEmpty(errormsg))
                {
                    await _context.Set<Transaction>().AddAsync(tranItem);
                    count_success++;
                    _logger.Warning(tranItem.ToString());
                }
                else 
                {
                    errorCollection.Add(count, errormsg);
                    _logger.Warning("Incorrect values for item: " + count + ". " + errormsg);
                    break;
                }
                count++;
            }
            
            //_logger.Warning("finish loop");
            await _context.SaveChangesAsync();
            //TODO: produce a better response that indicates 
            // all entries uploaded
            // partial success and which entries failed
            response = (count_success > 0) ?
                Status.SetStatus(request.Count, 200, $"{count_success} entries uploaded successfully") :
                Status.SetStatus(request.Count, 200, "No data for request");


            return response;
        }

       

    }
}
