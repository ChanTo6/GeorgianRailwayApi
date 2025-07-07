using GeorgianRailwayApi.Models;

namespace GeorgianRailwayApi.Services.Token
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
