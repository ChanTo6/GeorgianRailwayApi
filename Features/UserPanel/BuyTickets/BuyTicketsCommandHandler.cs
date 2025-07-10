using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Services.TicketService;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GeorgianRailwayApi.Features.UserPanel.BuyTickets
{
    public class BuyTicketsCommandHandler : IRequestHandler<BuyTicketsCommand, TicketPurchaseResultDto>
    {
        private readonly ITicketService _ticketService;
        public BuyTicketsCommandHandler(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        public async Task<TicketPurchaseResultDto> Handle(BuyTicketsCommand request, CancellationToken cancellationToken)
        {
            var dto = new TicketPurchaseRequestDto
            {
                UserId = request.UserId,
                Tickets = request.Tickets 
            };
            return await _ticketService.BuyTicketsAsync(dto);
        }
    }
}
