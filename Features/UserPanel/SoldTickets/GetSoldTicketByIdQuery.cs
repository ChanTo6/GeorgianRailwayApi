using GeorgianRailwayApi.DTOs;
using MediatR;

namespace GeorgianRailwayApi.Features.UserPanel.SoldTickets
{
    public class GetSoldTicketByIdQuery : IRequest<SoldTicketDto>
    {
        public int TicketId { get; set; }
    }
}
