using System.Collections.Generic;
using MediatR;
using GeorgianRailwayApi.DTOs;

namespace GeorgianRailwayApi.Features.AdminPanel.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<List<UpdateRequestDto>>
    {
    }
}
