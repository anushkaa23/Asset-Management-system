using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data.Context;
using AssetManagement.Data.Entities;

namespace AssetManagement.Business.Services
{
    public class AuthService
    {
        private readonly AssetDbContext _context;

        public AuthService(AssetDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            
            if (user == null)
                return null;

            var hashedPassword = HashPassword(password);
            if (user.PasswordHash != hashedPassword)
                return null;

            // Update last login date
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task EnsureAdminExistsAsync(string username, string password)
        {
            var adminExists = await _context.Users.AnyAsync(u => u.Username == username);
            
            if (!adminExists)
            {
                var admin = new User
                {
                    Username = username,
                    PasswordHash = HashPassword(password),
                    FullName = "System Administrator",
                    CreatedDate = DateTime.UtcNow
                };

                _context.Users.Add(admin);
                await _context.SaveChangesAsync();
            }
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}