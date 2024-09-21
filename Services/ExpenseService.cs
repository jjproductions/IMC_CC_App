using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Models;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using ILogger = Serilog.ILogger;

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

        public Task<ExpenseDTO> GetExpenses(int id = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<CommonDTO> PostExpense(List<ExpenseRequest> request)
        {
            Transaction tranItem = null;
            CommonDTO response = new();
            ConsolidatedInfo consolidatedInfo = await DataOb.GetConsolidatedInfo(_context);

            
            foreach (var item in request)
            {
                tranItem = new()
                {
                    Amount = item.Amount,
                    Description = item.Description,
                    Memo = item.Memo,
                    PostDate = item.PostDate.ToUniversalTime(),
                    TransactionDate = item.TransactionDate.ToUniversalTime(),
                };


                if (consolidatedInfo.creditCard.ContainsValue(item.CardNumber))
                {
                    tranItem.CardId = consolidatedInfo.creditCard.First(t => t.Value == item.CardNumber).Key;
                }

                if (consolidatedInfo.type.ContainsValue(item.Type))
                {
                    tranItem.TypeId = consolidatedInfo.type.First(t=>t.Value == item.Type).Key; 
                }

                if (consolidatedInfo.category.ContainsValue(item.Category))
                {
                    tranItem.CategoryId = consolidatedInfo.category.First(t => t.Value == item.Category).Key;
                }

                await _context.Set<Transaction>().AddAsync(tranItem);
            }
            
            await _context.SaveChangesAsync();
            response = (request.Count > 0) ?
                Utility.SetStatus(request.Count, 200, "Success") :
                Utility.SetStatus(request.Count, 200, "No data for request");


            return response;
        }

       

    }
}
