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
            var adminPassword = "Admin@12345"; 
            var hashedPassword = HashPassword(adminPassword);

            
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
            if (adminUser == null)
            {
                
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
<<<<<<< HEAD
               
=======
             
>>>>>>> 73621f9 (final)
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
