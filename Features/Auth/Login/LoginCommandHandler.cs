using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeorgianRailwayApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GeorgianRailwayApi.Services.Token;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        public LoginCommandHandler(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }
        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var hashedPassword = HashPassword(request.Password);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == hashedPassword, cancellationToken);
            if (user == null)
                return null;
            return new LoginResponseDto { Token = _jwtService.GenerateToken(user) };
        }
        private string HashPassword(string password)
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
