namespace AccountService.Core.Entities
{
    public class AccessLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public AccessType AccessType { get; set; }
        public int AccountId { get; set; }
        public string IpAddress { get; set; }
        public string Device { get; set; }
        public string UserAgent { get; set; }
        public string Location { get; set; }

        // Navigation property
        public Account Account { get; set; }
    }

    public enum AccessType
    {
        View,
        Download,
        Edit,
        Export
    }
}