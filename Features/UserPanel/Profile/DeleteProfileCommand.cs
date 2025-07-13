using MediatR;

namespace GeorgianRailwayApi.Features.UserPanel.Profile
{
    public class DeleteProfileCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}