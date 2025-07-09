using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Repositories.TicketRepositoryFile;
using MediatR;

namespace GeorgianRailwayApi.Features.UserPanel.SoldTickets
{
    public class GetSoldTicketByIdQueryHandler : IRequestHandler<GetSoldTicketByIdQuery, SoldTicketDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public GetSoldTicketByIdQueryHandler(ITicketRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public async Task<SoldTicketDto> Handle(GetSoldTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetSoldTicketByIdAsync(request.TicketId);
            return ticket == null ? null : _mapper.Map<SoldTicketDto>(ticket);
        }
    }
}
