using AccountService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountService.Core.Interfaces
{
    public interface ISecurityEventRepository
    {
        Task<IEnumerable<SecurityEvent>> GetByAccountIdAsync(int accountId);
        Task<SecurityEvent> CreateAsync(SecurityEvent securityEvent);
    }
}