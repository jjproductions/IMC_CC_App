using IMC_CC_App.DTO;

namespace IMC_CC_App
{
    public static class Utility
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
