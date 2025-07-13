using MediatR;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Features.UserPanel.Profile
{
    public class GetProfileQuery : IRequest<UpdateRequestDto>
    {
        public int UserId { get; set; }
    }
}