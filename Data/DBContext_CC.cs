using IMC_CC_App.Models;
using Microsoft.EntityFrameworkCore;

namespace IMC_CC_App.Data
{
    public class DbContext_CC : DbContext
    {
        public DbContext_CC(DbContextOptions<DbContext_CC> options) : base(options) { }

        public DbSet<CreditCard> CreditCards => Set<CreditCard>();
        public DbSet<Models.Type> Types => Set<Models.Type>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Roles> Roles => Set<Roles>();
        public DbSet<UserAuth> UserAuths => Set<UserAuth>();
        public DbSet<Users> Users => Set<Users>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
    }
}
