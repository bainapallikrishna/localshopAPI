using LocalShop.Domain.Models;
using LocalShop.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using LocalShop.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;

namespace LocalShop.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request);
        Task<bool> RegisterAsync(LoginRequest request, string role = "User");
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        Task<bool> RequestOtpAsync(string username);
        Task<LoginResponse?> VerifyOtpAsync(string username, string otp);
    }

    public class AuthService : IAuthService
    {
        private readonly LocalShopDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IMemoryCache _memoryCache;

        public AuthService(LocalShopDbContext context, IJwtService jwtService, IMemoryCache memoryCache)
        {
            _context = context;
            _jwtService = jwtService;
            _memoryCache = memoryCache;
        }

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return null;

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return null;

            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
            var token = _jwtService.GenerateToken(user.Username, user.Email, roles);

            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Roles = roles,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // This should match the JWT expiry
            };
        }

        public async Task<bool> RegisterAsync(LoginRequest request, string role = "User")
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return false;

            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return false;

            var user = new User
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                Email = request.Email ?? $"{request.Username}@example.com"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Get the role
            var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == role);
            if (roleEntity == null)
                return false;

            // Assign role to user
            var userRole = new UserRole
            {
                UserId = user.UserId,
                RoleId = roleEntity.RoleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return true;
        }

        public string HashPassword(string password)
        {
            //using var sha256 = SHA256.Create();
            //var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            //return Convert.ToBase64String(hashedBytes);
            return password;
        }

        public bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }

        public async Task<bool> RequestOtpAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return false;

            var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            _memoryCache.Set($"otp:{username}", otp, TimeSpan.FromMinutes(5));

            // TODO: Integrate SMS/email provider. For now, we just log/store.
            return true;
        }

        public async Task<LoginResponse?> VerifyOtpAsync(string username, string otp)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(otp)) return null;

            if (!_memoryCache.TryGetValue<string>($"otp:{username}", out var expected) || expected != otp)
            {
                return null;
            }

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
            var token = _jwtService.GenerateToken(user.Username, user.Email, roles);

            _memoryCache.Remove($"otp:{username}");

            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Roles = roles,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }
    }
} 