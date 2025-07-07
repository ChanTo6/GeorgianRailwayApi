using GeorgianRailwayApi.Models;
using MediatR;
using System.Collections.Generic;

namespace GeorgianRailwayApi.Features.UserPanel.SearchTrainsInfo
{
    public class SearchTrainsQuery
    {
        public class Query : IRequest<List<Train>>
        {
            public string Source { get; set; }
            public string Destination { get; set; }
        }
    }
}
