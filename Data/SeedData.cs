using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GeorgianRailwayApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GeorgianRailwayApi.Data
{
    public static class SeedData
    {
        public static async Task SeedAdminUserAsync(ApplicationDbContext context)
        {
            var adminEmail = "admin@georgianrailway.local";
            var adminPassword = "Admin@12345"; // Change this after first login
            var hashedPassword = HashPassword(adminPassword);

            // Try to find the admin user by email
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
            if (adminUser == null)
            {
                // Create default admin user if not exists
                adminUser = new User
                {
                    Email = adminEmail,
                    Password = hashedPassword,
                    Role = UserRole.Admin,
                    IsVerified = true
                };
                context.Users.Add(adminUser);
            }
            else
            {
                // Ensure the user has Admin role, correct password, and is verified
                adminUser.Password = hashedPassword;
                adminUser.Role = UserRole.Admin;
                adminUser.IsVerified = true;
                context.Users.Update(adminUser);
            }
            await context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
