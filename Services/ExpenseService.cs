using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Models;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using ILogger = Serilog.ILogger;
using IMC_CC_App.Utility;
using System.Text.Json;

namespace IMC_CC_App.Services
{
    public class ExpenseService : IExpense
    {
        private readonly ILogger _logger;
        private readonly DbContext_CC _context;
        private readonly IConfiguration _configuration;
        // private static Dictionary<string, int> _category;
        // private static Dictionary<string, int> _type;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

        public ExpenseService(DbContext_CC context, ILogger logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public Task<ExpenseDTO> GetExpensesAsync(int id = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<CommonDTO> PostExpenseAsync(List<ExpenseRequest> request)
        {
            //_logger.Warning("entering ExpenseService");
            Transaction? tranItem = null;
            CommonDTO response = new();
            ConsolidatedInfo consolidatedInfo = await DataOb.GetConsolidatedInfo(_context);
            //_logger.Warning("ExpenseService");
            Dictionary<int, string> errorCollection = new Dictionary<int, string>();
            List<int> cardIds = [];
            string errormsg = "";
            int count = 1;
            int count_success = 0;
            NotiificationUtilityRequest utilityRequest = new NotiificationUtilityRequest
            {
                cardIds = new List<int>(),
                sendToAdmin = false,
                status = StatusCategory.NEW
            };

            //_logger.Warning("start loop");
            try
            {
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

                    //_logger.Warning($"Card Number: {item.CardNumber}");
                    if (consolidatedInfo.creditCard.ContainsValue(item.CardNumber))
                    {
                        tranItem.CardId = consolidatedInfo.creditCard.First(t => t.Value == item.CardNumber).Key;
                        _logger.Warning($"tranItem ID: {tranItem.CardId}");
                        _logger.Warning($"cards collection:` {JsonSerializer.Serialize(cardIds, _jsonSerializerOptions)}");
                        if (!cardIds.Contains(item.CardNumber))
                            cardIds.Add(item.CardNumber);
                    }
                    else
                        errormsg = $"{errormsg} - card number: {item.CardNumber} does not exist;";

                    if (consolidatedInfo.type.ContainsValue(item.Type))
                    {
                        tranItem.TypeId = consolidatedInfo.type.First(t => t.Value == item.Type).Key;
                    }
                    else
                        errormsg = $"{errormsg} - type: {item.Type} does not exist;";

                    if (consolidatedInfo.category.ContainsValue(item.Category))
                    {
                        tranItem.CategoryId = consolidatedInfo.category.First(t => t.Value == item.Category).Key;
                    }
                    else if (item.Category == "")
                    {
                        tranItem.CategoryId = consolidatedInfo.category.First(t => t.Value == "UNKNOWN").Key;
                        _logger.Warning($"Category {item.Category} is empty, setting to {tranItem.CategoryId}");
                    }
                    else
                        errormsg = $"{errormsg} - category: {item.Category} does not exist;";

                    if (string.IsNullOrEmpty(errormsg))
                    {
                        await _context.Set<Transaction>().AddAsync(tranItem);
                        count_success++;
                        _logger.Warning(tranItem?.ToString() ?? "tranItem is null");
                    }
                    else
                    {
                        errorCollection.Add(count, errormsg);
                        _logger.Error($"Incorrect values for item #: {count}. {JsonSerializer.Serialize(errorCollection, _jsonSerializerOptions)}");
                        throw new Exception($"Incorrect values for item #: {count}. {JsonSerializer.Serialize(errorCollection, _jsonSerializerOptions)}");
                    }
                    count++;
                }

                await _context.SaveChangesAsync();
                response = Status.SetStatus(request.Count, 200, $"{count_success} entries uploaded successfully");
                // Notify staff of newly updated expenses
                _logger.Warning($"Successfully uploaded {count_success} expenses.  Sending notifications to {cardIds.Count} staff.");
                utilityRequest.cardIds = cardIds;
                var notifications = new Notifications(_context, _logger, _configuration).SendNotificationAsync(utilityRequest);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in ExpenseService:PostExpenseAsync: successfully uploaded {count - 1} expenses {ex.Message}");
                Status.SetStatus(request.Count, 500, JsonSerializer.Serialize(errorCollection, _jsonSerializerOptions));
            }
            finally
            {


            }
            //TODO: produce a better response that indicates 
            // all entries uploaded
            // partial success and which entries failed



            return response;
        }

    }
}
