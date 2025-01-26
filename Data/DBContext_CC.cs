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
            modelBuilder.Entity<Report_SP>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }

        //Function Models
        public DbSet<AuthorizedUsersDB> AuthUsers => Set<AuthorizedUsersDB>();
        public DbSet<StatmentsDB> Statments => Set<StatmentsDB>();
        public DbSet<UserDataDB> UserDataDB => Set<UserDataDB>();
        public DbSet<Report_SP> ReportDB => Set<Report_SP>();
        public DbSet<ReportStatments_SP> ReportStatementsDB => Set<ReportStatments_SP>();

        // Method to call the PostgreSQL function
        public async Task<List<AuthorizedUsersDB>> GetAuthUserInfo(string? email = null)
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
        public async Task<List<UserDataDB>> GetUserInfo(string? email = null)
        {
            if (email != null)
                return await UserDataDB
                .FromSqlRaw("SELECT * FROM get_user('" + email + "')")
                .ToListAsync();
            else
                return await UserDataDB
                .FromSqlRaw("SELECT * FROM get_all_users()")
                .ToListAsync();
        }

        //Return statements only for a user
        public async Task<List<StatmentsDB>> GetStatements(string? email = null)
        {
            return await Statments
                .FromSqlRaw("SELECT * FROM get_user_statements('" + email + "')").ToListAsync();
        }

        // Admin use cases - 
        // Statements by User ID
        // Get all statements
        public async Task<List<StatmentsDB>> GetStatements(int? id = null, bool getAllStatements = false)
        {
            if (getAllStatements)  //Return all statements
                return await Statments
                    .FromSqlRaw("SELECT * FROM get_all_statements()").ToListAsync();

            _logger.Warning($"context GetStatements::id={id}");
            //Return statements only for a user
            return await Statments
                .FromSqlRaw($"SELECT * FROM get_user_statements_by_card({id})").ToListAsync();
        }

        // Get all statements assigned to a report
        public async Task<List<ReportStatments_SP>> GetReportStatements(int reportId)
        {
            return await ReportStatementsDB
                .FromSqlRaw($"SELECT * FROM get_statements_by_report({reportId})").ToListAsync();
        }

        // Get all reports in Pending / Returned status
        public async Task<List<Report_SP>> GetReports_NotSubmitted(int cardNumber)
        {
            return await ReportDB
                .FromSqlRaw($"SELECT * FROM get_reports_by_card_open({cardNumber})").ToListAsync();
        }

        // Update statements within a report
        public async Task<Boolean> UpdateStatements(int? rptId, List<StatementUpdateRequest> request)
        {
            //string statements = "";

            foreach (var item in request)
            {
                //statements = JsonSerializer.Serialize(request, _jsonSerializerOptions);
                
                //statements = $"'{statements}'::jsonb";
                _logger.Warning($"Calling Update_Statements: {rptId} - {item.Id} - {item.Memo}");

                await Database.ExecuteSqlRawAsync($"SELECT update_statement({rptId}, {item.Id}, '{item.Category}', '{item.Type}', '{item.Memo}')");
                //await Database.ExecuteSqlRawAsync($"SELECT update_statements({rptId}, '{statements}'::jsonb)");
            }

            SaveChangesAsync();
            return true;
        }
    }
}
