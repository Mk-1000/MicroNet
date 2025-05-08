using AccountService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountService.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account> GetByIdAsync(int id);
        Task<Account> GetByEmailAsync(string email);
        Task<Account> GetByGoogleIdAsync(string googleId);
        Task<Account> CreateAsync(Account account);
        Task<Account> UpdateAsync(Account account);
        Task<bool> DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}