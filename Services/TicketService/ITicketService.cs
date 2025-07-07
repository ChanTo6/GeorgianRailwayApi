using System.Threading.Tasks;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Services.TicketService
{
    public interface ITicketService
    {
        Task<TicketPurchaseResultDto> BuyTicketsAsync(TicketPurchaseRequestDto request);
    }
}