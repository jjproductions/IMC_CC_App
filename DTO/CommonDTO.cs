namespace IMC_CC_App.DTO
{
    public class CommonDTO
    {
        private int _statusCode = -1;
        private int _count = 0;
        public string? StatusMessage { get; set; }
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

    public class SASTokenRefreshRequest
    {
        public string? ContainerName { get; set; }
        public required string BlobName { get; set; }
    }

    public enum Permission
    {
        Viewer = 0,
        Edit = 1,
        Admin = 2,
        SuperAdmin = 3
    }

    public enum StatementStatus
    {
        OPEN,
        NOTOPEN
    }

    public enum StatusCategory
    {
        PENDING = 0,
        SUBMITTED = 1,
        APPROVED = 2,
        RETURNED = 3,
        DELETED = 4,
        NEW = 5
    }
}
