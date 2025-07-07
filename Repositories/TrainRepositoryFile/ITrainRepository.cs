using System.Threading.Tasks;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Models;

namespace GeorgianRailwayApi.Repositories.TrainRepositoryFile
{
    public interface ITrainRepository
    {
        Task<Train> GetTrainByIdAsync(int trainId);
        Task UpdateTrainAsync(Train train);
        Task<TrainResponseDto> AddTrainAsync(TrainRequestDto trainDto);
    }
}