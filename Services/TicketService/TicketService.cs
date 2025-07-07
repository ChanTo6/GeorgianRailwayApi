using System;
using System.Linq;
using System.Threading.Tasks;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Models;
using GeorgianRailwayApi.Repositories.TicketRepositoryFile;
using GeorgianRailwayApi.Repositories.TrainRepositoryFile;
using GeorgianRailwayApi.Exceptions;

namespace GeorgianRailwayApi.Services.TicketService
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly ITrainRepository _trainRepo;
        public TicketService(ITicketRepository ticketRepo, ITrainRepository trainRepo)
        {
            _ticketRepo = ticketRepo;
            _trainRepo = trainRepo;
        }
        public async Task<TicketPurchaseResultDto> BuyTicketsAsync(TicketPurchaseRequestDto request)
        {
            var ticketsGroupedByTrain = GroupTicketsByTrain(request.Tickets);

            await EnsureTrainsExistAndHaveAvailableSeatsAsync(ticketsGroupedByTrain);
            await EnsureSeatsAreAvailableAsync(request.Tickets);
            await ProcessTicketBookingAsync(request.UserId, request.Tickets);

            return new TicketPurchaseResultDto
            {
                Success = true,
                Message = "Tickets have been successfully purchased."
            };
        }

        private Dictionary<int, List<TicketPurchaseDto>> GroupTicketsByTrain(IEnumerable<TicketPurchaseDto> tickets)
        {
            return tickets
                .GroupBy(ticket => ticket.TrainId)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        private async Task EnsureTrainsExistAndHaveAvailableSeatsAsync(Dictionary<int, List<TicketPurchaseDto>> ticketsByTrain)
        {
            foreach (var entry in ticketsByTrain)
            {
                int trainId = entry.Key;
                var tickets = entry.Value;

                var train = await _trainRepo.GetTrainByIdAsync(trainId);
                if (train == null)
                    throw new NotFoundException($"Train with ID {trainId} was not found.");

                int availableSeats = train.TotalSeats - train.BookedSeats;
                if (tickets.Count > availableSeats)
                {
                    throw new DomainException(
                        $"Insufficient seats on train '{train.Name}'. Requested: {tickets.Count}, Available: {availableSeats}.",
                        "InsufficientSeats"
                    );
                }
            }
        }

        private async Task EnsureSeatsAreAvailableAsync(IEnumerable<TicketPurchaseDto> tickets)
        {
            foreach (var ticket in tickets)
            {
                bool isAlreadyBooked = await _ticketRepo.IsSeatBookedAsync(ticket.TrainId, ticket.SeatNumber);
                if (isAlreadyBooked)
                {
                    throw new DomainException(
                        $"Seat {ticket.SeatNumber} on train ID {ticket.TrainId} is already booked.",
                        "SeatAlreadyBooked"
                    );
                }
            }
        }

        private async Task ProcessTicketBookingAsync(int userId, IEnumerable<TicketPurchaseDto> tickets)
        {
            foreach (var ticket in tickets)
            {
                var train = await _trainRepo.GetTrainByIdAsync(ticket.TrainId);
                if (train == null || train.BookedSeats >= train.TotalSeats)
                    continue; 

                var ticketEntity = new Ticket
                {
                    UserId = userId,
                    TrainId = ticket.TrainId,
                    SeatNumber = ticket.SeatNumber,
                    IsBooked = true,
                    PurchaseDate = DateTime.UtcNow
                };

                train.BookedSeats++;

                await _ticketRepo.AddTicketAsync(ticketEntity);
                await _trainRepo.UpdateTrainAsync(train);
            }
        }

    }
}
