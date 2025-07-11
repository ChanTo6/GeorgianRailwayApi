using GeorgianRailwayApi.DTOs;
using MediatR;

namespace GeorgianRailwayApi.Features.UserPanel.SoldTickets
{
    public class GetSoldTicketsByUserIdQuery : IRequest<List<SoldTicketDto>>
    {
        public int UserId { get; set; }
    }
}
