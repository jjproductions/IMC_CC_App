using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Data
{
    public class DbContext_CC(DbContextOptions<DbContext_CC> options, ILogger logger) : DbContext(options)
    {
        private readonly ILogger _logger = logger;

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
            modelBuilder.Entity<UserDataDB>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }

        //Function Models
        public DbSet<AuthorizedUsersDB> AuthUsers => Set<AuthorizedUsersDB>();
        public DbSet<StatmentsDB> Statments => Set<StatmentsDB>();
        public DbSet<UserDataDB> UserDataDB => Set<UserDataDB>();
        



        // Method to call the PostgreSQL function
        public async Task<List<AuthorizedUsersDB>> GetAuthUserInfo(string? email=null)
        {
            List<AuthorizedUsersDB>? response = 
                email == null ? 
                (
                    //Return user info for all users
                    response = await AuthUsers
                    .FromSqlRaw("SELECT * FROM get_all_authusers()")
                    .ToListAsync()
                ) : (
                    //Return single user info
                    response = await AuthUsers
                    .FromSqlRaw("SELECT * FROM authuser('" + email + "')")
                    .ToListAsync()
                );
            return response;
        }

        // Get info from all active users
        public async Task<List<UserDataDB>> GetUserInfo(string? email=null)
        {
            if (email != null)
                return await UserDataDB
                .FromSqlRaw("SELECT * FROM get_user('"+ email + "')")
                .ToListAsync();
            else
                return await UserDataDB
                .FromSqlRaw("SELECT * FROM get_all_users()")
                .ToListAsync();
        }

        public async Task<List<StatmentsDB>> GetStatements(string? email=null)
        {
            //Return statements only for a user
            return await Statments
                .FromSqlRaw("SELECT * FROM get_user_statements('" + email + "')").ToListAsync();
        }

        // Admin use cases - 
        // Statements by User ID
        // Get all statements
        public async Task<List<StatmentsDB>> GetStatements(int? id=null, bool getAllStatements = false)
        {
            if (getAllStatements)  //Return all statements
                return await Statments
                    .FromSqlRaw("SELECT * FROM get_all_statements()").ToListAsync();

            _logger.Warning($"context GetStatements::id={id}");
            //Return statements only for a user
            return await Statments
                .FromSqlRaw("SELECT * FROM get_user_statements_by_card(" + id + ")").ToListAsync();
        }

    }
}
