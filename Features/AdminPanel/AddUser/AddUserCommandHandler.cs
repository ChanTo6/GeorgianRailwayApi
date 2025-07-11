using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeorgianRailwayApi.Features.AdminPanel.AddUser
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, RegisterResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AddUserCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<RegisterResponseDto> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.UserDto.Email, cancellationToken))
                throw new System.Exception("User with this email already exists.");

            var user = new User
            {
                Email = request.UserDto.Email,
                Password = request.UserDto.Password, 
                Role = Enum.TryParse<UserRole>(request.UserDto.Role, true, out var role) ? role : UserRole.User,
                IsVerified = true 
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<RegisterResponseDto>(user);
        }
    }
}
