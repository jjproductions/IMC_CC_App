using System.Security.Claims;
using IMC_CC_App.DTO;
using Microsoft.AspNetCore.Authorization;
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
}
