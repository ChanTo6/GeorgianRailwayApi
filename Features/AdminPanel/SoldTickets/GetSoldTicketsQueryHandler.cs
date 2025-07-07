using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace GeorgianRailwayApi.Features.Admin.SoldTickets
{
    public class GetSoldTicketsQueryHandler : IRequestHandler<GetSoldTicketsQuery, List<SoldTicketDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetSoldTicketsQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<SoldTicketDto>> Handle(GetSoldTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _context.Tickets
                .Where(t => t.IsBooked)
                .Include(t => t.Train)
                .Include(t => t.User)
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<SoldTicketDto>>(tickets);
        }
    }
}
