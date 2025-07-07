using MediatR;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Features.Auth.Login
{
    public class LoginCommand : IRequest<LoginResponseDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
