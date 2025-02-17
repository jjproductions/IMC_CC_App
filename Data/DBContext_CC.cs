using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ILogger = Serilog.ILogger;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace IMC_CC_App.Data
{
    public class DbContext_CC(DbContextOptions<DbContext_CC> options, ILogger logger) : DbContext(options)
    {
        private readonly ILogger _logger = logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();

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
            modelBuilder.Entity<ReportStatments_SP>().HasNoKey();
            modelBuilder.Entity<UpdateReport_SP>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }

        //Function Models
        public DbSet<AuthorizedUsersDB> AuthUsers => Set<AuthorizedUsersDB>();
        public DbSet<StatmentsDB> Statments => Set<StatmentsDB>();
        public DbSet<UserDataDB> UserDataDB => Set<UserDataDB>();
        public DbSet<Report_SP> ReportDB => Set<Report_SP>();
        public DbSet<ReportStatments_SP> ReportStatementsDB => Set<ReportStatments_SP>();
        public DbSet<UpdateReport_SP> ReportUpdateResponse => Set<UpdateReport_SP>();

        //public DbSet<int> UpdateStDB => Set<int>();

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
                .FromSqlInterpolated($"SELECT * FROM get_user_statements_by_card({id})").ToListAsync();
        }

        // Get all statements assigned to a report
        public async Task<List<ReportStatments_SP>> GetReportStatements(int reportId)
        {
            var response = await ReportStatementsDB
                .FromSqlInterpolated($"SELECT * FROM get_statements_by_report({reportId})").AsNoTracking().ToListAsync();

            string statements = JsonSerializer.Serialize(response, _jsonSerializerOptions);
            _logger.Warning($"GetReportStatements - {statements} statements returned for report {reportId} :: ");
            return response;
        }

        // Get all reports in Pending / Returned status
        public async Task<List<Report_SP>> GetReports_NotSubmitted(int cardNumber)
        {
            return await ReportDB
                .FromSqlInterpolated($"SELECT * FROM get_reports_by_card_open({cardNumber})").AsNoTracking().ToListAsync();
        }

        public async Task<List<ReportStatments_SP>> UpdateReportStatements(int rptId, StatementUpdateRequestDTO request)
        {
            List<ReportStatments_SP> response = new();
            // First clear report id from all statements identified in the request
            if (request.ItemsToDelete != null && request.ItemsToDelete.Length > 0)
            {
                string statements = string.Join(',', request.ItemsToDelete);
                _logger.Warning($"UpdateReportStatements_DB: Calling update_remove_rptids_statement to remove statement ids: {statements} from report {rptId}");
                // add rptId so the function will only remove if equal to the report id
                await Database.ExecuteSqlInterpolatedAsync($"SELECT update_remove_rptids_statement({statements})");
                await SaveChangesAsync();
            }

            // Now update the statements with the new information
            foreach (var item in request.Statements)
            {
                //statements = JsonSerializer.Serialize(request, _jsonSerializerOptions);

                //statements = $"'{statements}'::jsonb";
                _logger.Warning($"UpdateReportStatements_DB: Calling update_statement: {rptId} - {item.Id} - {item.Memo} - {item.Category} - {item.Type} - {item.ReceiptUrl}");

                await Database.ExecuteSqlInterpolatedAsync($"SELECT update_statement({rptId}, {item.Id}, {item.Category}, {item.Type}, {item.Memo}, {item.ReceiptUrl})");
            }

            await SaveChangesAsync();
            this.ChangeTracker.Clear();

            response = await ReportStatementsDB
                .FromSqlInterpolated($"SELECT * FROM get_statements_by_report({rptId})")
                .AsNoTracking()
                .ToListAsync();

            _logger.Warning($"UpdateReportStatements_DB - {JsonSerializer.Serialize(response, _jsonSerializerOptions)} statements returned for report {rptId}");
            return response;
        }
        // Update statements within a report
        // change this to return the updated data ***
        public async Task<Boolean> UpdateStatements(int? rptId, StatementUpdateRequestDTO request)
        {

            // First clear report id from all statements identified in the request
            Console.WriteLine($"UpdateStatements: # of items to remove - {request.ItemsToDelete?.Length}");
            _logger.Warning($"UpdateStatements: # of items to remove - {request.ItemsToDelete?.Length}");
            if (request.ItemsToDelete != null && request.ItemsToDelete.Length > 0)
            {
                string statements = string.Join(',', request.ItemsToDelete);
                _logger.Warning($"Calling update_remove_rptids_statement to remove statement ids: {statements} from report {rptId}");
                // add rptId so the function will only remove if equal to the report id
                await Database.ExecuteSqlInterpolatedAsync($"SELECT update_remove_rptids_statement({statements})");
                await SaveChangesAsync();
            }

            // Now update the statements with the new information
            foreach (var item in request.Statements)
            {
                //statements = JsonSerializer.Serialize(request, _jsonSerializerOptions);

                //statements = $"'{statements}'::jsonb";
                _logger.Warning($"Calling update_statement: {rptId} - {item.Id} - {item.Memo} - {item.Category} - {item.Type} - {item.ReceiptUrl}");

                await Database.ExecuteSqlInterpolatedAsync($"SELECT update_statement({rptId}, {item.Id}, {item.Category}, {item.Type}, {item.Memo}, {item.ReceiptUrl})");
                await SaveChangesAsync();
            }


            return true;
        }

        public async Task<UpdateReport_SP> UpdateReport(int rptId, string? reportMemo, string status)
        {
            try
            {
                var result = await ReportUpdateResponse
                        .FromSqlInterpolated($@"SELECT * FROM update_report_by_id({rptId}, {status}, {reportMemo})")
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                _logger.Warning($"DBCC:Update Report by Id SP - {result?.status} :: {result?.name}` ");
                await SaveChangesAsync();
                return result;

            }
            catch (Exception err)
            {
                _logger.Error($"Create Report by Card SP - {err}");
            }
            return null;
        }

        public async Task<bool> DeleteReport(int reportId, int[] itemsToDelete)
        {
            // Don't believe i need this check.  If no statements to delete, delete the report
            // if (itemsToDelete == null || itemsToDelete.Length == 0)
            // {
            //     _logger.Warning($"Delete Report operation failed because statement id parameter is empty - {reportId}");
            //     return false;
            // }

            string expenseIds = itemsToDelete != null ? string.Join(',', itemsToDelete) : string.Empty;
            try
            {
                using (var command = Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT delete_report(@reportId, @expenseIds)";
                    command.CommandType = CommandType.Text;

                    // Add parameters to prevent SQL injection
                    var reportIdParam = command.CreateParameter();
                    reportIdParam.ParameterName = "reportId";
                    reportIdParam.Value = reportId;

                    var reportDeletedItemsParam = command.CreateParameter();
                    reportDeletedItemsParam.ParameterName = "expenseIds";
                    reportDeletedItemsParam.Value = expenseIds;

                    command.Parameters.Add(reportIdParam);
                    command.Parameters.Add(reportDeletedItemsParam);

                    await Database.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                    await SaveChangesAsync();
                    await Database.CloseConnectionAsync();
                }
            }
            catch (Exception err)
            {
                _logger.Error($"DeleteReport by ID SP - {err}");
                return false;
            }
            return true;
        }

        public async Task<int> CreateReport(ReportNewRequest request)
        {
            int result = 0;
            try
            {
                _logger.Warning($"CreateReport - {request.Name} : {request.CardNumber} : {request.Status.ToString()}");
                using (var command = Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT create_report(@card_number, @report_name, @report_memo, @report_status)";
                    command.CommandType = CommandType.Text;

                    // Add parameters to prevent SQL injection
                    var cardNumberParam = command.CreateParameter();
                    cardNumberParam.ParameterName = "card_number";
                    cardNumberParam.Value = request.CardNumber;

                    var reportNameParam = command.CreateParameter();
                    reportNameParam.ParameterName = "report_name";
                    reportNameParam.Value = request.Name;

                    var reportMemoParam = command.CreateParameter();
                    reportMemoParam.ParameterName = "report_memo";
                    reportMemoParam.Value = request.Memo;

                    var reportStatusParam = command.CreateParameter();
                    reportStatusParam.ParameterName = "report_status";
                    reportStatusParam.Value = request.Status.ToString();

                    command.Parameters.Add(cardNumberParam);
                    command.Parameters.Add(reportNameParam);
                    command.Parameters.Add(reportMemoParam);
                    command.Parameters.Add(reportStatusParam);


                    await Database.OpenConnectionAsync();
                    result = Convert.ToInt32(await command.ExecuteScalarAsync());
                    _logger.Warning($"CreateReport: New report created - {request.Name} with ID: {result}");
                    await SaveChangesAsync();
                    await Database.CloseConnectionAsync();
                }
            }
            catch (Exception err)
            {
                _logger.Error($"CreateReport: New report creation failed in SP - {err}");
            }
            return result;
        }
    }
}
