using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GeorgianRailwayApi.Features.AdminPanel.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, RegisterResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public UpdateUserCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<RegisterResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user == null)
                return null;
            if (!string.IsNullOrWhiteSpace(request.Email))
                user.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.Password))
                user.Password = HashPassword(request.Password);
            if (!string.IsNullOrWhiteSpace(request.Role) && Enum.TryParse<UserRole>(request.Role, true, out var role))
                user.Role = role;
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
