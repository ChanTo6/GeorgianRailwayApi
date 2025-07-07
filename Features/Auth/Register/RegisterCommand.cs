using MediatR;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Features.Auth.Register
{
    public class RegisterCommand : IRequest<RegisterResponseDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
    }
}
