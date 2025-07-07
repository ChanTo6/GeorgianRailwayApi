using MediatR;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Features.AdminPanel.AddTrain
{
    public class AddTrainCommand : IRequest<TrainResponseDto>
    {
        public TrainRequestDto TrainDto { get; set; }
    }
}
