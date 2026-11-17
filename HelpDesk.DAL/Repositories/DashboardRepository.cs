
using HelpDesk.BLL.DTOs.Dashboard;
using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DAL.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetStatsAsync(
            CancellationToken cancellationToken = default)
        {
            var stats = new DashboardStatsDto
            {
                TotalTickets = await _context.Tickets
                    .CountAsync(cancellationToken),

                OpenTickets = await _context.Tickets
                    .CountAsync(t => t.Status == "Open", cancellationToken),

                InProgressTickets = await _context.Tickets
                    .CountAsync(t => t.Status == "In Progress", cancellationToken),

                ResolvedTickets = await _context.Tickets
                    .CountAsync(t => t.Status == "Resolved", cancellationToken),

                TotalEmployees = await (
                    from userRole in _context.UserRoles
                    join role in _context.Roles
                        on userRole.RoleId equals role.Id
                    where role.Name == "Employee"
                    select userRole.UserId
                ).Distinct().CountAsync(cancellationToken),

                TotalTechnicians = await (
                    from userRole in _context.UserRoles
                    join role in _context.Roles
                        on userRole.RoleId equals role.Id
                    where role.Name == "Technician"
                    select userRole.UserId
                ).Distinct().CountAsync(cancellationToken)
            };

            return stats;
        }


        public async Task<IReadOnlyList<TicketActivityDto>> GetTicketActivityAsync(
            int days = 7,
            CancellationToken cancellationToken = default)
        {
            if (days < 1 || days > 90)
                throw new ArgumentOutOfRangeException(nameof(days));

            var today = DateTime.UtcNow.Date;
            var startDate = today.AddDays(-(days - 1));
            var endDate = today.AddDays(1);

            var tickets = await _context.Tickets
                .AsNoTracking()
                .Where(t =>
                    (t.CreatedAt >= startDate && t.CreatedAt < endDate) ||
                    (t.ClosedAt.HasValue &&
                     t.ClosedAt.Value >= startDate &&
                     t.ClosedAt.Value < endDate))
                .Select(t => new
                {
                    t.CreatedAt,
                    t.ClosedAt
                })
                .ToListAsync(cancellationToken);

            var activity = Enumerable.Range(0, days)
                .Select(i =>
                {
                    var date = startDate.AddDays(i);

                    return new TicketActivityDto
                    {
                        Date = date,

                        CreatedTickets = tickets.Count(t =>
                            t.CreatedAt.Date == date),

                        ResolvedTickets = tickets.Count(t =>
                            t.ClosedAt.HasValue &&
                            t.ClosedAt.Value.Date == date)
                    };
                })
                .ToList();

            return activity;
        }

    }
}
