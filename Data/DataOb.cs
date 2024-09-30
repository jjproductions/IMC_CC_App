using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using IMC_CC_App.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;



namespace IMC_CC_App.Data
{
    public class DataOb
    {
        
        private static Dictionary<int, int>? CardInfo { get; set; }
        private static Dictionary<int, string>? CategoryInfo { get; set; }
        private static Dictionary<int, string>? TypeInfo { get; set; }
        private static Dictionary<int, UserCollection>? UserInfo { get; set; }


        public static async Task<Dictionary<int,UserCollection>> GetUserInfo(DbContext_CC _context)
        {
            if (UserInfo == null)
            {
                Dictionary<int, UserCollection> results = [];
                UserCollection user = new();

                var response = await _context.Set<Models.Users>().ToListAsync().ConfigureAwait(false);

                if (response.Any())
                {
                    foreach (var type in response)
                    {
                        user.Name = type.Name;
                        user.Email = type.Email;
                        results.Add(type.Id, user);
                    }
                    UserInfo = results;
                }

                return results;
            }
            else { return UserInfo; }
        }

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
                    TypeInfo = results;
                }
               
                return results;
            }
            else { return TypeInfo; }
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
                    CategoryInfo = results;
                }
                
                return results;
            }
            else { return CategoryInfo; }
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
                    CardInfo = results;
                }
                
                return results;
            }
            else { return CardInfo; }
        }
    }

    public class UserCollection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class ConsolidatedInfo
    {
        public Dictionary<int, string> category { get; set; }
        public Dictionary<int, string> type { get; set; }
        public Dictionary <int, int> creditCard { get; set; }
        public Dictionary<int, string> user { get; set; }

    }
}
