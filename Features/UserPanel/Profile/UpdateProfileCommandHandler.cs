using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using GeorgianRailwayApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace GeorgianRailwayApi.Features.UserPanel.Profile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, RegisterResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public UpdateProfileCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<RegisterResponseDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine(request.Email, request.Password);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user == null)
                return null;
            if (!string.IsNullOrWhiteSpace(request.Email))
                user.Email = request.Email;
          
            if (!string.IsNullOrWhiteSpace(request.Password))
                user.Password = HashPassword(request.Password);
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