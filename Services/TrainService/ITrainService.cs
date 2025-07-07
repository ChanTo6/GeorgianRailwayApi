using System.Threading.Tasks;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Services.TrainService
{
    public interface ITrainService
    {
        Task<TrainResponseDto> AddTrainAsync(TrainRequestDto trainDto);
    }
}
