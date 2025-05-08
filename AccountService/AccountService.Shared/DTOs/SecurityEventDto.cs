namespace AccountService.Shared.DTOs
{
    public class SecurityEventDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }
        public string Device { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }
    }
}
