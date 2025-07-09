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
using GeorgianRailwayApi.Services.Email;

namespace GeorgianRailwayApi.Features.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        public RegisterCommandHandler(ApplicationDbContext context, IMapper mapper, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
        }
        public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
                return null;

            var user = _mapper.Map<User>(request);
            user.Password = HashPassword(request.Password);
            user.IsVerified = false;
            user.VerificationPin = GeneratePin();
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            await _emailService.SendEmailAsync(user.Email, "Your Registration PIN", $"Your verification PIN is: {user.VerificationPin}");
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
        private string GeneratePin()
        {
            var rng = new Random();
            return rng.Next(100000, 999999).ToString();
        }
    }
}