using AutoMapper;
using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GeorgianRailwayApi.Features.UserPanel.Profile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UpdateRequestDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetProfileQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<UpdateRequestDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            if (user == null) return null;
            return _mapper.Map<UpdateRequestDto>(user);
        }
    }
}