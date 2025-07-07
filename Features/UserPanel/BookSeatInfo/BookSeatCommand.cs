using GeorgianRailwayApi.Models;
using MediatR;

namespace GeorgianRailwayApi.Features.UserPanel.BookSeatInfo
{
    public class BookSeatCommand
    {
        public class Command : IRequest<Result>
        {
            public int TrainId { get; set; }
        }
    }
}
