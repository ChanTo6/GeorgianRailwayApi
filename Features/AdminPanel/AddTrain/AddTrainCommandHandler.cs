using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Repositories.TrainRepositoryFile;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GeorgianRailwayApi.Features.AdminPanel.AddTrain
{
    public class AddTrainCommandHandler : IRequestHandler<AddTrainCommand, TrainResponseDto>
    {
        private readonly ITrainRepository _trainRepository;
        public AddTrainCommandHandler(ITrainRepository trainRepository)
        {
            _trainRepository = trainRepository;
        }
        public async Task<TrainResponseDto> Handle(AddTrainCommand request, CancellationToken cancellationToken)
        {
            return await _trainRepository.AddTrainAsync(request.TrainDto);
        }
    }
}
