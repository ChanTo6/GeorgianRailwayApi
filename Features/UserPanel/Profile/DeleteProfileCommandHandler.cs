using System.Threading;
using System.Threading.Tasks;
using GeorgianRailwayApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeorgianRailwayApi.Features.UserPanel.Profile
{
    public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        public DeleteProfileCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user == null)
                return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}