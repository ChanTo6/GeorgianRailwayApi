using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GeorgianRailwayApi.DTOs;
using AutoMapper;

namespace GeorgianRailwayApi.Features.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public RegisterCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
                return null;

            var user = _mapper.Map<User>(request);
            user.Password = HashPassword(request.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<RegisterResponseDto>(user);
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