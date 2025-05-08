namespace AccountService.Core.Entities
{
    public class SecurityEvent
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public SecurityEventType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }
        public string Device { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }

        // Navigation property
        public Account Account { get; set; }
    }

    public enum SecurityEventType
    {
        Login,
        Logout,
        PasswordChange,
        AccountLock,
        AccountUnlock,
        EmailChange,
        PhoneChange,
        AccountDeletion,
        Other
    }
}