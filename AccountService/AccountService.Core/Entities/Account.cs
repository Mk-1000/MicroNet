using System;
using System.Collections.Generic;

namespace AccountService.Core.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? GoogleId { get; set; }
        public string Phone { get; set; }
        public AccountType Type { get; set; }
        public DateTime LastLoginAt { get; set; }

        // Set default values in code for CreatedAt and UpdatedAt
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<SecurityEvent> SecurityEvents { get; set; } = new List<SecurityEvent>();
        public ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();
    }


    public enum AccountType
    {
        Admin,
        User
    }
}