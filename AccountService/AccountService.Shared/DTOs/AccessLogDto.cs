namespace AccountService.Shared.DTOs
{
    public class AccessLogDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string AccessType { get; set; }
        public string IpAddress { get; set; }
        public string Device { get; set; }
        public string UserAgent { get; set; }
        public string Location { get; set; }
    }
}