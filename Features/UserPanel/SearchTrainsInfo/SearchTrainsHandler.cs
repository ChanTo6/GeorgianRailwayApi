using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static GeorgianRailwayApi.Features.UserPanel.SearchTrainsInfo.SearchTrainsQuery;

namespace GeorgianRailwayApi.Features.UserPanel.SearchTrainsInfo
{
    public class SearchTrainsHandler
    {
        public class Handler : IRequestHandler<Query, List<Train>>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Train>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Trains
                    .Where(t => t.Source == request.Source && t.Destination == request.Destination)
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
