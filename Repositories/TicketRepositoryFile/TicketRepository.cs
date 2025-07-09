using System.Collections.Generic;
using System.Threading.Tasks;
using GeorgianRailwayApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GeorgianRailwayApi.Repositories.TicketRepositoryFile
{
    public class TicketRepository : ITicketRepository
    {
        private readonly Data.ApplicationDbContext _context;
        public TicketRepository(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsSeatBookedAsync(int trainId, int seatNumber)
        {
            return await _context.Tickets.AnyAsync(t => t.TrainId == trainId && t.SeatNumber == seatNumber && t.IsBooked);
        }
        public async Task AddTicketAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
        }
        public async Task<List<Ticket>> GetSoldTicketsAsync()
        {
            return await _context.Tickets.Include(t => t.Train).Include(t => t.User).Where(t => t.IsBooked).ToListAsync();
        }

        public async Task<Ticket> GetSoldTicketByIdAsync(int ticketId)
        {
            return await _context.Tickets
                .Include(t => t.Train)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.IsBooked);
        }
    }
}