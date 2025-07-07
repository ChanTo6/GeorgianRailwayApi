using System.Threading.Tasks;
using GeorgianRailwayApi.Models;

namespace GeorgianRailwayApi.Repositories.UserRepositoryFile
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int userId);
    }
}