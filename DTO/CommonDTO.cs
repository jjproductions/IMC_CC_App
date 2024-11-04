namespace IMC_CC_App.DTO
{
    public class CommonDTO
    {
        private int _statusCode = -1;
        private int _count = 0;
        public string? StatusMessage {get; set;}
        public int StatusCode
        {
            get => _statusCode;
            set => _statusCode = value;
        }
        public int Count
        {
            get => _count;
            set => _count = value;
        }
    }

    public static class CustomClaims
    {
        public const string Role = "Role";
    }

    public enum Permission
    {
        Viewer = 0,
        Edit = 1,
        Admin = 2,
        SuperAdmin = 3
    }
}
