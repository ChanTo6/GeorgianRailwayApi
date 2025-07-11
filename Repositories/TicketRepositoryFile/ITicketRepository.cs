using System.Collections.Generic;
using System.Threading.Tasks;
using GeorgianRailwayApi.Models;

namespace GeorgianRailwayApi.Repositories.TicketRepositoryFile
{
    public interface ITicketRepository
    {
        Task<bool> IsSeatBookedAsync(int trainId, int seatNumber);
        Task AddTicketAsync(Ticket ticket);
        Task<List<Ticket>> GetSoldTicketsAsync();
        Task<List<Ticket>> GetSoldTicketsByUserIdAsync(int userId);

    }
}