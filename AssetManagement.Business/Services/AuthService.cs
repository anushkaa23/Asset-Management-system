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

        /// <summary>
        /// Validates user credentials for login
        /// </summary>
        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null; // User not found

            var hashedPassword = HashPassword(password);
            if (user.PasswordHash != hashedPassword)
                return null; // Wrong password

            // Update last login date
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        public async Task<User?> RegisterUserAsync(string username, string password, string fullName = "")
        {
            // Check if username already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Username '{username}' is already taken");
            }

            // Validate username
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                throw new ArgumentException("Username must be at least 3 characters long");
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                throw new ArgumentException("Password must be at least 8 characters long");
            }

            // Create new user
            var hashedPassword = HashPassword(password);
            var newUser = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                FullName = string.IsNullOrWhiteSpace(fullName) ? username : fullName,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        /// <summary>
        /// Checks if a username is already taken
        /// </summary>
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        /// <summary>
        /// Ensures admin user exists (called on startup)
        /// </summary>
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

        /// <summary>
        /// Hashes password using SHA256
        /// </summary>
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}