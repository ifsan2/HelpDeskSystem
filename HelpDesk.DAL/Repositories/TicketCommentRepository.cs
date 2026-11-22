
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
            string requestingUserId,
            bool includeInternal,
            CancellationToken cancellationToken = default)
        {
            if (ticketId <= 0 ||
                string.IsNullOrWhiteSpace(requestingUserId))
            {
                throw new ArgumentException(
                    "Invalid ticket or user information.");
            }

            // Verify that the requesting user exists and is active.
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Id == requestingUserId && u.IsActive,
                    cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Your account is inactive or does not exist.");
            }

            // Verify that the ticket exists.
            var ticket = await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    t => t.Id == ticketId,
                    cancellationToken);

            if (ticket == null)
            {
                throw new InvalidOperationException(
                    "Ticket does not exist.");
            }

            // Verify the user's actual Admin role through Identity.
            bool isAdmin = await (
                from userRole in _context.UserRoles
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where userRole.UserId == requestingUserId
                      && role.Name == "Admin"
                select role.Id
            ).AnyAsync(cancellationToken);

            // Only Admins can retrieve internal comments.
            if (includeInternal && !isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to view internal comments.");
            }

            // Non-admin users must own or be assigned to the ticket.
            bool hasTicketAccess =
                ticket.CreatedById == requestingUserId ||
                ticket.AssignedToId == requestingUserId;

            if (!isAdmin && !hasTicketAccess)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to view this ticket's comments.");
            }

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
            if (comment.TicketId <= 0 ||
                string.IsNullOrWhiteSpace(comment.UserId))
            {
                throw new ArgumentException(
                    "Invalid ticket or user information.");
            }

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
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Id == comment.UserId && u.IsActive,
                    cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "User does not exist or is inactive.");
            }

            // Retrieve the user's actual Identity roles.
            var userRoles = await (
                from userRole in _context.UserRoles
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where userRole.UserId == comment.UserId
                select role.Name
            ).ToListAsync(cancellationToken);

            bool isAdmin = userRoles.Contains("Admin");
            bool isEmployee = userRoles.Contains("Employee");
            bool isTechnician = userRoles.Contains("Technician");

            // Only Admins can create internal comments.
            if (comment.IsInternal && !isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only Admins can create internal comments.");
            }

            // Admins can comment on any ticket.
            // Employees can comment on their own tickets.
            // Technicians can comment on tickets assigned to them.
            bool hasTicketAccess =
                isAdmin ||
                (isEmployee && ticket.CreatedById == comment.UserId) ||
                (isTechnician && ticket.AssignedToId == comment.UserId);

            if (!hasTicketAccess)
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
