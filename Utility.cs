using System.Text;
using System.Text.Json;
using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using ILogger = Serilog.ILogger;

namespace IMC_CC_App.Utility
{
    public static class Status
    {
        public static CommonDTO SetStatus(int count, int code, string message)
        {
            CommonDTO response = new();
            if (code > 0 && !string.IsNullOrEmpty(message))
            {
                response.StatusMessage = message;
                response.StatusCode = code;
                response.Count = count;
            }

            return response;
        }
    }

    public class Notifications
    {
        private readonly DbContext_CC _context;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public Notifications(DbContext_CC context, ILogger logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // create a request object containing cardids, status and sendToAdmin flag, report name
        public async Task<bool> SendNotificationAsync(NotiificationUtilityRequest request)
        {
            try
            {
                List<NotificationAPIRequest> notificationRequests = new();
                if (request.sendToAdmin)
                {
                    notificationRequests.Add(new NotificationAPIRequest
                    {
                        cardId = request.cardIds[0],
                        email = _configuration["FinanceAdmin:Email"]?.ToString() ?? "",
                        phoneNumber = "", // _configuration["FinanceAdmin:Phone"]?.ToString() ?? "", // Add this line
                        emailInfo = new EmailInfo
                        {
                            subject = $"Review pending for card {request.cardIds[0]}",
                            body = $"A report has been uploaded for your review. {_configuration["IMCUIUrl"]?.ToString() ?? ""} \n\n" +
                                "This is an automated message. Please do not reply."
                        }
                    });
                }
                else
                {
                    // call users SP to get email addresses
                    List<UserInfo_SP> userInfo = await _context.GetUserInfoByCard(request.cardIds);
                    _logger.Warning($"User Info: {JsonSerializer.Serialize(userInfo)}");
                    string emailBody = "";
                    string emailSubject = "";
                    // call email service to send email
                    foreach (var item in userInfo)
                    {
                        // switch statement by Status
                        // case 1: new expenses
                        // case 2: report Returned
                        // case 3: report Approved
                        switch (request.status)
                        {
                            case StatusCategory.RETURNED:
                                emailSubject = "Report Returned";
                                emailBody = $"{item.name}, \n Your report has been returned for further action. {_configuration["IMCUIUrl"]?.ToString() ?? ""} \n\n" +
                                    "This is an automated message. Please do not reply.";
                                break;
                            case StatusCategory.APPROVED:
                                emailSubject = "Report Approved";
                                emailBody = $"{item.name}, \n Your report has been approved. {_configuration["IMCUIUrl"]?.ToString() ?? ""} \n\n" +
                                    "This is an automated message. Please do not reply.";
                                break;
                            default:
                                emailSubject = "New Expenses ready for review";
                                emailBody = $"{item.name}, \n New expenses await your review. {_configuration["IMCUIUrl"]?.ToString() ?? ""} \n\n" +
                                    "This is an automated message. Please do not reply.";
                                break;
                        }
                        notificationRequests.Add(new NotificationAPIRequest
                        {
                            cardId = item.card_number,
                            email = item.email,
                            phoneNumber = "", // Add this line
                            emailInfo = new EmailInfo
                            {
                                subject = emailSubject,
                                body = emailBody
                            }
                        });
                    }
                }
                // Send notifications
                return await SendAsync(notificationRequests, false, new APIInfo
                {
                    APIKey
                        = _configuration["SendNotificationAPI:APIKey"]?.ToString() ?? "",
                    APIURL = _configuration["SendNotificationAPI:Url"]?.ToString() ?? ""
                }, _logger);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send notifications");
                return false;
            }
        }
        private static async Task<bool> SendAsync(List<NotificationAPIRequest> userinfo, bool includeSMS, APIInfo apiInfo, Serilog.ILogger logger)
        {
            // Send notifications
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Add("api-key", apiInfo.APIKey);

                    logger.Warning($"Sending notifications to {userinfo.Count} users: {JsonSerializer.Serialize(apiInfo)} :: {JsonSerializer.Serialize(userinfo)}");
                    var content = new StringContent(JsonSerializer.Serialize(userinfo), Encoding.UTF8, "application/json");
                    var results = await httpClient.PostAsync(apiInfo.APIURL, content);

                    if (results.IsSuccessStatusCode)
                    {
                        logger.Information("Notifications sent at {time}", DateTimeOffset.UtcNow);
                        return true;
                    }
                    else
                    {
                        var errorMessage = await results.Content.ReadAsStringAsync();
                        logger.Error($"Failed to send notifications: {errorMessage}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Failed to send notifications");
                    return false;
                }
            }
        }
    }
}
