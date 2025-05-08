using AccountService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountService.Core.Interfaces
{
    public interface IAccessLogRepository
    {
        Task<IEnumerable<AccessLog>> GetByAccountIdAsync(int accountId);
        Task<AccessLog> CreateAsync(AccessLog accessLog);
    }
}