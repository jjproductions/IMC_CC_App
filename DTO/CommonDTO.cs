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
}
