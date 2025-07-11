using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeorgianRailwayApi.Features.AdminPanel.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UpdateRequestDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetAllUsersQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<UpdateRequestDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users.ToListAsync(cancellationToken);
            return _mapper.Map<List<UpdateRequestDto>>(users);
        }
    }
}
