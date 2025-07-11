using MediatR;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Features.AdminPanel.AddUser
{
    public class AddUserCommand : IRequest<RegisterResponseDto>
    {
        public RegisterRequestDto UserDto { get; set; }
    }
}
