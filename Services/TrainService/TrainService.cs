using System.Threading.Tasks;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Repositories.TrainRepositoryFile;

namespace GeorgianRailwayApi.Services.TrainService
{
    public class TrainService : ITrainService
    {
        private readonly ITrainRepository _trainRepository;
        public TrainService(ITrainRepository trainRepository)
        {
            _trainRepository = trainRepository;
        }
        public async Task<TrainResponseDto> AddTrainAsync(TrainRequestDto trainDto)
        {
            return await _trainRepository.AddTrainAsync(trainDto);
        }
    }
}
