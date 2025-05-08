using AccountService.Core.Entities;
using AccountService.Core.Interfaces;
using AccountService.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace AccountService.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ISecurityEventRepository _securityEventRepository;
        private readonly IAccessLogRepository _accessLogRepository;
        private readonly IConfiguration _configuration;

        public AccountService(
            IAccountRepository accountRepository,
            ISecurityEventRepository securityEventRepository,
            IAccessLogRepository accessLogRepository,
            IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _securityEventRepository = securityEventRepository;
            _accessLogRepository = accessLogRepository;
            _configuration = configuration;
        }

        public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return accounts.Select(MapToAccountDto);
        }

        public async Task<AccountDto> GetAccountByIdAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
                return null;

            return MapToAccountDto(account);
        }

        public async Task<AccountDto> CreateAccountAsync(AccountCreateDto accountDto, string password)
        {
            if (await _accountRepository.EmailExistsAsync(accountDto.Email))
                throw new InvalidOperationException("Email already exists");

            var account = new Account
            {
                FullName = accountDto.FullName,
                Email = accountDto.Email,
                Phone = accountDto.Phone,
                Type = AccountType.User,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            // In a real system, you'd store these password credentials securely

            var createdAccount = await _accountRepository.CreateAsync(account);
            return MapToAccountDto(createdAccount);
        }

        public async Task<AccountDto> UpdateAccountAsync(int id, AccountUpdateDto accountDto)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
                return null;

            account.FullName = accountDto.FullName ?? account.FullName;
            account.Phone = accountDto.Phone ?? account.Phone;
            account.UpdatedAt = DateTime.UtcNow;

            var updatedAccount = await _accountRepository.UpdateAsync(account);
            return MapToAccountDto(updatedAccount);
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
                return false;

            await _securityEventRepository.CreateAsync(new SecurityEvent
            {
                AccountId = id,
                Type = SecurityEventType.AccountDeletion,
                Timestamp = DateTime.UtcNow,
                Details = "Account deleted"
            });

            return await _accountRepository.DeleteAsync(id);
        }

        public async Task<AuthResponseDto> AuthenticateAsync(LoginDto loginDto, string ipAddress, string device)
        {
            var account = await _accountRepository.GetByEmailAsync(loginDto.Email);
            if (account == null)
                return null;

            // Validate password (in a real system)
            // if (!VerifyPasswordHash(loginDto.Password, storedHash, storedSalt))
            //    return null;

            // In this example, we're assuming password is valid for simplicity

            account.LastLoginAt = DateTime.UtcNow;
            await _accountRepository.UpdateAsync(account);

            await _securityEventRepository.CreateAsync(new SecurityEvent
            {
                AccountId = account.Id,
                Type = SecurityEventType.Login,
                IpAddress = ipAddress,
                Device = device,
                Timestamp = DateTime.UtcNow,
                Details = "Login successful"
            });

            // Generate JWT Token
            var token = GenerateJwtToken(account);
            var refreshToken = GenerateRefreshToken();

            // In a real system, you'd store the refresh token

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600, // 1 hour
                User = MapToAccountDto(account)
            };
        }

        public async Task<AuthResponseDto> GoogleAuthenticateAsync(GoogleLoginDto googleLoginDto, string ipAddress, string device)
        {
            // In a real system, you'd validate the Google token
            // For this example, we'll assume the token is valid and contains the Google ID

            string googleId = "google-id-from-token"; // Placeholder
            string email = "google-email-from-token"; // Placeholder
            string name = "Google User"; // Placeholder

            var account = await _accountRepository.GetByGoogleIdAsync(googleId);

            if (account == null)
            {
                // Create new account for first-time Google login
                account = new Account
                {
                    FullName = name,
                    Email = email,
                    GoogleId = googleId,
                    Type = AccountType.User,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                account = await _accountRepository.CreateAsync(account);
            }
            else
            {
                account.LastLoginAt = DateTime.UtcNow;
                await _accountRepository.UpdateAsync(account);
            }

            await _securityEventRepository.CreateAsync(new SecurityEvent
            {
                AccountId = account.Id,
                Type = SecurityEventType.Login,
                IpAddress = ipAddress,
                Device = device,
                Timestamp = DateTime.UtcNow,
                Details = "Google login successful"
            });

            // Generate JWT Token
            var token = GenerateJwtToken(account);
            var refreshToken = GenerateRefreshToken();

            // In a real system, you'd store the refresh token

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600, // 1 hour
                User = MapToAccountDto(account)
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            // In a real system, you'd validate the refresh token from storage
            // For this example, we'll assume the refresh token is valid

            int accountId = 1; // Placeholder
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                return null;

            // Generate new JWT Token
            var token = GenerateJwtToken(account);
            var newRefreshToken = GenerateRefreshToken();

            // In a real system, you'd update the refresh token in storage

            await _securityEventRepository.CreateAsync(new SecurityEvent
            {
                AccountId = account.Id,
                Type = SecurityEventType.Login,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                Details = "Token refresh successful"
            });

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = newRefreshToken,
                ExpiresIn = 3600, // 1 hour
                User = MapToAccountDto(account)
            };
        }

        public async Task<bool> LogoutAsync(int accountId, string ipAddress, string device)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                return false;

            // In a real system, you'd invalidate the refresh token

            await _securityEventRepository.CreateAsync(new SecurityEvent
            {
                AccountId = accountId,
                Type = SecurityEventType.Logout,
                IpAddress = ipAddress,
                Device = device,
                Timestamp = DateTime.UtcNow,
                Details = "Logout successful"
            });

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int accountId, string currentPassword, string newPassword, string ipAddress, string device)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                return false;

            // In a real system, you'd verify the current password and update to the new one
            // For this example, we'll assume the current password is valid

            await _securityEventRepository.CreateAsync(new SecurityEvent
            {
                AccountId = accountId,
                Type = SecurityEventType.PasswordChange,
                IpAddress = ipAddress,
                Device = device,
                Timestamp = DateTime.UtcNow,
                Details = "Password changed successfully"
            });

            return true;
        }

        public async Task<IEnumerable<SecurityEventDto>> GetSecurityEventsAsync(int accountId)
        {
            var events = await _securityEventRepository.GetByAccountIdAsync(accountId);
            return events.Select(e => new SecurityEventDto
            {
                Id = e.Id,
                Type = e.Type.ToString(),
                Timestamp = e.Timestamp,
                IpAddress = e.IpAddress,
                Device = e.Device,
                Location = e.Location,
                Details = e.Details
            });
        }

        public async Task<IEnumerable<AccessLogDto>> GetAccessLogsAsync(int accountId)
        {
            var logs = await _accessLogRepository.GetByAccountIdAsync(accountId);
            return logs.Select(l => new AccessLogDto
            {
                Id = l.Id,
                Timestamp = l.Timestamp,
                AccessType = l.AccessType.ToString(),
                IpAddress = l.IpAddress,
                Device = l.Device,
                UserAgent = l.UserAgent,
                Location = l.Location
            });
        }

        public async Task AddAccessLogAsync(int accountId, AccessType accessType, string ipAddress, string device, string userAgent, string location)
        {
            await _accessLogRepository.CreateAsync(new AccessLog
            {
                AccountId = accountId,
                AccessType = accessType,
                IpAddress = ipAddress,
                Device = device,
                UserAgent = userAgent,
                Location = location,
                Timestamp = DateTime.UtcNow
            });
        }

        #region Helper Methods

        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Name, account.FullName),
                new Claim(ClaimTypes.Role, account.Type.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i])
                    return false;
            }

            return true;
        }

        private AccountDto MapToAccountDto(Account account)
        {
            return new AccountDto
            {
                Id = account.Id,
                FullName = account.FullName,
                Email = account.Email,
                Phone = account.Phone,
                Type = account.Type.ToString(),
                LastLoginAt = account.LastLoginAt,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }

        #endregion

    }
}