using AccountService.Core.Entities;
using AccountService.Core.Interfaces;
using AccountService.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Core.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
        Task<AccountDto> GetAccountByIdAsync(int id);
        Task<AccountDto> CreateAccountAsync(AccountCreateDto accountDto, string password);
        Task<AccountDto> UpdateAccountAsync(int id, AccountUpdateDto accountDto);
        Task<bool> DeleteAccountAsync(int id);
        Task<AuthResponseDto> AuthenticateAsync(LoginDto loginDto, string ipAddress, string device);
        Task<AuthResponseDto> GoogleAuthenticateAsync(GoogleLoginDto googleLoginDto, string ipAddress, string device);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress);
        Task<bool> LogoutAsync(int accountId, string ipAddress, string device);
        Task<bool> ChangePasswordAsync(int accountId, string currentPassword, string newPassword, string ipAddress, string device);
        Task<IEnumerable<SecurityEventDto>> GetSecurityEventsAsync(int accountId);
        Task<IEnumerable<AccessLogDto>> GetAccessLogsAsync(int accountId);
        Task AddAccessLogAsync(int accountId, AccessType accessType, string ipAddress, string device, string userAgent, string location);
    }
}
