using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Repositories.TicketRepositoryFile;
using MediatR;

namespace GeorgianRailwayApi.Features.UserPanel.SoldTickets
{
    public class GetSoldTicketsByUserIdQueryHandler : IRequestHandler<GetSoldTicketsByUserIdQuery, List<SoldTicketDto>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public GetSoldTicketsByUserIdQueryHandler(ITicketRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public async Task<List<SoldTicketDto>> Handle(GetSoldTicketsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketRepository.GetSoldTicketsByUserIdAsync(request.UserId);
            return _mapper.Map<List<SoldTicketDto>>(tickets);
        }
    }

}
