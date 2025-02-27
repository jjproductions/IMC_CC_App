namespace IMC_CC_App.DTO
{
    public class NotificationAPIRequest
    {
        public required int cardId { get; set; }
        public required string email { get; set; }
        public required string phoneNumber { get; set; }
        public required EmailInfo emailInfo { get; set; }
    }

    public class EmailInfo
    {
        public required string subject { get; set; }

        public required string body { get; set; }
    }

    public class APIInfo
    {
        public required string APIKey { get; set; }
        public required string APIURL { get; set; }
    }

    public class NotiificationUtilityRequest
    {
        public required List<int> cardIds { get; set; } = [];
        public required bool sendToAdmin { get; set; } = false;
        public StatusCategory? status { get; set; }
    }

}