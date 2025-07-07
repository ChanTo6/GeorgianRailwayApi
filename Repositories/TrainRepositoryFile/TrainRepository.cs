using System.Threading.Tasks;
using GeorgianRailwayApi.Models;
using Microsoft.EntityFrameworkCore;
using GeorgianRailwayApi.DTOs;
using AutoMapper;

namespace GeorgianRailwayApi.Repositories.TrainRepositoryFile
{
    public class TrainRepository : ITrainRepository
    {
        private readonly Data.ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public TrainRepository(Data.ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Train> GetTrainByIdAsync(int trainId)
        {
            return await _context.Trains.FindAsync(trainId);
        }
        public async Task UpdateTrainAsync(Train train)
        {
            _context.Trains.Update(train);
            await _context.SaveChangesAsync();
        }
        public async Task<TrainResponseDto> AddTrainAsync(TrainRequestDto trainDto)
        {
            var train = _mapper.Map<Train>(trainDto);
            _context.Trains.Add(train);
            await _context.SaveChangesAsync();
            return _mapper.Map<TrainResponseDto>(train);
        }
    }
}