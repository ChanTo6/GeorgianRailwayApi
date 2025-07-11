using MediatR;

namespace GeorgianRailwayApi.Features.AdminPanel.DeleteUser
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
