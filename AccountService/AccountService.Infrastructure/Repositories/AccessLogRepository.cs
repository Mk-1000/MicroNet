using AccountService.Core.Entities;
using AccountService.Core.Interfaces;
using AccountService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.Repositories
{
    public class AccessLogRepository : IAccessLogRepository
    {
        private readonly AccountDbContext _context;

        public AccessLogRepository(AccountDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccessLog>> GetByAccountIdAsync(int accountId)
        {
            return await _context.AccessLogs
                .Where(a => a.AccountId == accountId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<AccessLog> CreateAsync(AccessLog accessLog)
        {
            accessLog.Timestamp = DateTime.UtcNow;

            _context.AccessLogs.Add(accessLog);
            await _context.SaveChangesAsync();

            return accessLog;
        }
    }
}