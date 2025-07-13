using MediatR;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Features.UserPanel.Profile
{
    public class UpdateProfileCommand : IRequest<RegisterResponseDto>
    {
        public int Id { get; set; }
        public string? Email { get; set; } // Make nullable
        public string? Password { get; set; } // Make nullable
    }
}