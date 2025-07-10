using AutoMapper;
using GeorgianRailwayApi.Models;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Features.Auth.Register;

namespace GeorgianRailwayApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, RegisterResponseDto>();
            CreateMap<RegisterCommand, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => MapRole(src.Role)));

            // Ticket -> SoldTicketDto
            CreateMap<Ticket, SoldTicketDto>()
                .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TrainName, opt => opt.MapFrom(src => src.Train.Name))
                .ForMember(dest => dest.TrainId, opt => opt.MapFrom(src => src.TrainId))
                .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.SeatNumber))
                .ForMember(dest => dest.BuyerEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.BuyerId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => src.PurchaseDate));

            // Train <-> Train DTOs
            CreateMap<Train, TrainResponseDto>();
            CreateMap<TrainRequestDto, Train>();


        }

        private static UserRole MapRole(string role)
        {
            return Enum.TryParse<UserRole>(role, true, out var parsedRole) ? parsedRole : UserRole.User;
        }
    }
}
