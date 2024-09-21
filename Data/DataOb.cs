using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using IMC_CC_App.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;



namespace IMC_CC_App.Data
{
    public class DataOb
    {
        
        private static Dictionary<int, int> CardInfo { get; set; }
        private static Dictionary<int, string> CategoryInfo { get; set; }
        private static Dictionary<int, string> TypeInfo { get; set; }


        public static async Task<ConsolidatedInfo> GetConsolidatedInfo(DbContext_CC _context)
        {
            ConsolidatedInfo response = new();

            response.creditCard = await GetCardInfo(_context);
            response.category = await GetCategoryInfo(_context);
            response.type = await GetTypeInfo(_context);

            return response;
        }


        private static async Task<Dictionary<int, string>> GetTypeInfo(DbContext_CC _context)
        {
            if (TypeInfo == null)
            {
                Dictionary<int, string> results = new Dictionary<int, string>();
                var response = await _context.Set<Models.Type>().ToListAsync().ConfigureAwait(false);

                if (response.Any())
                {
                    foreach (var type in response)
                    {
                        results.Add(type.Id, type.Name);
                    }
                }
                return results;
            }
            else { return null; }
        }

        private static async Task<Dictionary<int, string>> GetCategoryInfo(DbContext_CC _context)
        {
            if (CategoryInfo == null)
            {
                Dictionary<int, string> results = new Dictionary<int, string>();
                var response = await _context.Set<Category>().ToListAsync().ConfigureAwait(false);

                if (response.Any())
                {
                    foreach (var category in response)
                    {
                        results.Add(category.Id, category.Name);
                    }
                }
                return results;
            }
            else { return null; }
        }

        private static async Task<Dictionary<int, int>> GetCardInfo(DbContext_CC _context)
        {
            if (CardInfo == null)
            {
                Dictionary<int, int> results = new Dictionary<int, int>();
                var response = await _context.Set<CreditCard>().ToListAsync().ConfigureAwait(false);

                if (response.Any())
                {
                    foreach (var card in response)
                    {
                        results.Add(card.Id, card.CardNumber);
                    }
                }
                return results;
            }
            else { return null; }
        }
    }



    public class ConsolidatedInfo
    {
        public Dictionary<int, string> category { get; set; }
        public Dictionary<int, string> type { get; set; }

        public Dictionary <int, int> creditCard { get; set; }

    }
}
