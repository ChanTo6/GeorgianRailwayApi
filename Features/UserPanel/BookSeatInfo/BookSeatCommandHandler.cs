using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using static GeorgianRailwayApi.Features.UserPanel.BookSeatInfo.BookSeatCommand;

namespace GeorgianRailwayApi.Features.UserPanel.BookSeatInfo
{
    public class BookSeatCommandHandler
    {
        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var train = await _context.Trains
                    .FirstOrDefaultAsync(t => t.TrainId == request.TrainId, cancellationToken);

                if (train == null)
                    return new Result { Success = false, Message = "Train not found." };

                if (train.TotalSeats <= 0)
                    return new Result { Success = false, Message = "No available seats." };

                train.TotalSeats -= 1;

                await _context.SaveChangesAsync(cancellationToken);

                return new Result
                {
                    Success = true,
                    Message = "Seat booked successfully.",
                    SeatNumber = train.TotalSeats + 1 
                };
            }
        }
    }
}
