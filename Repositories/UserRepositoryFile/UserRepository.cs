using System.Collections.Generic;
using System.Threading.Tasks;
using GeorgianRailwayApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GeorgianRailwayApi.Repositories.UserRepositoryFile
{
    public class UserRepository : IUserRepository
    {
        private readonly Data.ApplicationDbContext _context;
        public UserRepository(Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }
}