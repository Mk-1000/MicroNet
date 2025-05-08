using AccountService.Core.Entities;
using AccountService.Core.Interfaces;
using AccountService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.Repositories
{
    public class SecurityEventRepository : ISecurityEventRepository
    {
        private readonly AccountDbContext _context;

        public SecurityEventRepository(AccountDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SecurityEvent>> GetByAccountIdAsync(int accountId)
        {
            return await _context.SecurityEvents
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.Timestamp)
                .ToListAsync();
        }

        public async Task<SecurityEvent> CreateAsync(SecurityEvent securityEvent)
        {
            securityEvent.Timestamp = DateTime.UtcNow;

            _context.SecurityEvents.Add(securityEvent);
            await _context.SaveChangesAsync();

            return securityEvent;
        }
    }
}