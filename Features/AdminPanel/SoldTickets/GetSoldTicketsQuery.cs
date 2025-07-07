using System.Collections.Generic;
using GeorgianRailwayApi.DTOs;
using MediatR;

namespace GeorgianRailwayApi.Features.Admin.SoldTickets
{
    public class GetSoldTicketsQuery : IRequest<List<SoldTicketDto>>
    {
    }
}
