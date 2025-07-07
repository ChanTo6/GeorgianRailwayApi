using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeorgianRailwayApi.Features.UserPanel
{
    public class GetAllTrains
    {
        public class Query : IRequest<List<Train>> { }
        public class Handler : IRequestHandler<Query, List<Train>>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<Train>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Trains.ToListAsync(cancellationToken);
            }
        }
    }
}
