using System.Collections.Generic;
using MediatR;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Features.UserPanel.BuyTickets
{
   
    public class BuyTicketsCommand : IRequest<TicketPurchaseResultDto>
    {
        public int UserId { get; set; }
        public List<TicketPurchaseDto> Tickets { get; set; }
    }
}
