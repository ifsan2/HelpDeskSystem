
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

        private async Task<List<string>> GetActiveUserRolesAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            return await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == userId
                      && user.IsActive
                select role.Name!
            ).Distinct().ToListAsync(cancellationToken);
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

            var userRoles = await GetActiveUserRolesAsync(
                requestingUserId,
                cancellationToken);

            if (userRoles.Count == 0)
            {
                throw new UnauthorizedAccessException(
                    "Your account is inactive, does not exist, or has no assigned role.");
            }

            bool isAdmin = userRoles.Contains("Admin");
            bool isEmployee = userRoles.Contains("Employee");
            bool isTechnician = userRoles.Contains("Technician");

            if (!isAdmin && !isEmployee && !isTechnician)
            {
                throw new UnauthorizedAccessException(
                    "You do not have a valid application role.");
            }

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

            if (includeInternal && !isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to view internal comments.");
            }

            bool hasTicketAccess =
                isAdmin ||
                (isEmployee && ticket.CreatedById == requestingUserId) ||
                (isTechnician && ticket.AssignedToId == requestingUserId);

            if (!hasTicketAccess)
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
            ArgumentNullException.ThrowIfNull(comment);

            if (comment.TicketId <= 0 ||
                string.IsNullOrWhiteSpace(comment.UserId))
            {
                throw new ArgumentException(
                    "Invalid ticket or user information.");
            }

            if (string.IsNullOrWhiteSpace(comment.Message))
            {
                throw new ArgumentException(
                    "Comment message is required.");
            }

            if (comment.Message.Length > 2000)
            {
                throw new ArgumentException(
                    "Comment message cannot exceed 2000 characters.");
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

            var userRoles = await GetActiveUserRolesAsync(
                comment.UserId,
                cancellationToken);

            if (userRoles.Count == 0)
            {
                throw new UnauthorizedAccessException(
                    "User does not exist, is inactive, or has no assigned role.");
            }

            bool isAdmin = userRoles.Contains("Admin");
            bool isEmployee = userRoles.Contains("Employee");
            bool isTechnician = userRoles.Contains("Technician");

            if (!isAdmin && !isEmployee && !isTechnician)
            {
                throw new UnauthorizedAccessException(
                    "User does not have a valid application role.");
            }

            if (comment.IsInternal && !isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only Admins can create internal comments.");
            }

            bool hasTicketAccess =
                isAdmin ||
                (isEmployee && ticket.CreatedById == comment.UserId) ||
                (isTechnician && ticket.AssignedToId == comment.UserId);

            if (!hasTicketAccess)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to comment on this ticket.");
            }

            comment.Message = comment.Message.Trim();

            await _context.TicketComments.AddAsync(
                comment,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
