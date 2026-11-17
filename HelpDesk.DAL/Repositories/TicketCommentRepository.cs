
using HelpDesk.BLL.DTOs.Tickets;
using HelpDesk.BLL.Entities;
using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DAL.Repositories
{
    public class TicketCommentRepository : ITicketCommentRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TicketCommentDto>> GetByTicketIdAsync(
            int ticketId,
            bool includeInternal,
            CancellationToken cancellationToken = default)
        {
            var query = _context.TicketComments
                .AsNoTracking()
                .Where(c => c.TicketId == ticketId);

            if (!includeInternal)
            {
                query = query.Where(c => !c.IsInternal);
            }

            return await query
                .OrderBy(c => c.CreatedAt)
                .Select(c => new TicketCommentDto
                {
                    Id = c.Id,
                    TicketId = c.TicketId,
                    UserId = c.UserId,

                    UserName = _context.Users
                        .Where(u => u.Id == c.UserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "Unknown User",

                    Message = c.Message,
                    CreatedAt = c.CreatedAt,
                    IsInternal = c.IsInternal
                })
                .ToListAsync(cancellationToken);
        }


        public async Task AddAsync(
            TicketComment comment,
            CancellationToken cancellationToken = default)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(
                    t => t.Id == comment.TicketId,
                    cancellationToken);

            if (ticket == null)
            {
                throw new InvalidOperationException(
                    "Ticket does not exist.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == comment.UserId && u.IsActive,
                    cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "User does not exist or is inactive.");
            }

            bool isTicketCreator = ticket.CreatedById == comment.UserId;
            bool isAssignedTechnician = ticket.AssignedToId == comment.UserId;

            if (!isTicketCreator && !isAssignedTechnician)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to comment on this ticket.");
            }

            await _context.TicketComments.AddAsync(
                comment,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
