using IMC_CC_App.DTO;
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

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure AuthorizedUsersDB to have no key
            modelBuilder.Entity<AuthorizedUsersDB>().HasNoKey();
            modelBuilder.Entity<StatmentsDB>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }

        //Function Models
        public DbSet<AuthorizedUsersDB> AuthUsers => Set<AuthorizedUsersDB>();
        public DbSet<StatmentsDB> Statments => Set<StatmentsDB>();



        // Method to call the PostgreSQL function
        public async Task<List<AuthorizedUsersDB>> GetAuthUserInfo(string email, bool getAllUsers = false)
        {
            if (getAllUsers)
                //Return user info for all users
                return await AuthUsers
                .FromSqlRaw("SELECT * FROM get_all_authusers()")
                .ToListAsync();

            //Return single user info
            return await AuthUsers
            .FromSqlRaw("SELECT * FROM authuser('" + email + "')")
            .ToListAsync();
        }

        public async Task<List<StatmentsDB>> GetStatements(string email, bool getAllStatements = false)
        {
            if (getAllStatements)  //Return all statements
                return await Statments
                    .FromSqlRaw("SELECT * FROM get_all_statements()").ToListAsync();

            //Return statements only for a user
            return await Statments
                .FromSqlRaw("SELECT * FROM get_user_statements('" + email + "')").ToListAsync();
        }
    }
}
