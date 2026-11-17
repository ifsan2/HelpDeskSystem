
using HelpDesk.BLL.DTOs.Tickets;
using HelpDesk.BLL.Entities;
using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DAL.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(
            Ticket ticket,
            CancellationToken cancellationToken = default)
        {
            await _context.Tickets.AddAsync(ticket, cancellationToken);

            var history = new TicketHistory
            {
                Ticket = ticket,
                UserId = ticket.CreatedById,
                Action = "Created",
                OldValue = null,
                NewValue = ticket.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TicketHistories.AddAsync(
                history,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task<string?> GetLastTicketNumberAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .AsNoTracking()
                .OrderByDescending(t => t.Id)
                .Select(t => t.TicketNumber)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<EmployeeTicketDto>> GetByEmployeeIdAsync(
            string employeeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.CreatedById == employeeId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new EmployeeTicketDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Title = t.Title,
                    CategoryName = t.Category.Name,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    AssignedTechnicianName = t.AssignedToId == null
                        ? null
                        : _context.Users
                            .Where(u => u.Id == t.AssignedToId)
                            .Select(u => u.FirstName + " " + u.LastName)
                            .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }


        public async Task<IReadOnlyList<EmployeeTicketDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new EmployeeTicketDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Title = t.Title,
                    CategoryName = t.Category.Name,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    AssignedTechnicianId = t.AssignedToId,
                    AssignedTechnicianName = t.AssignedToId == null
                        ? null
                        : _context.Users
                            .Where(u => u.Id == t.AssignedToId)
                            .Select(u => u.FirstName + " " + u.LastName)
                            .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }


        public async Task<TicketDetailsDto?> GetDetailsByIdAsync(
            int ticketId,
            string employeeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.Id == ticketId && t.CreatedById == employeeId)
                .Select(t => new TicketDetailsDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Title = t.Title,
                    Description = t.Description,
                    CategoryName = t.Category.Name,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    ClosedAt = t.ClosedAt,
                    AssignedTechnicianName = t.AssignedToId == null
                        ? null
                        : _context.Users
                            .Where(u => u.Id == t.AssignedToId)
                            .Select(u => u.FirstName + " " + u.LastName)
                            .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }


        // Admin can view any ticket by its ID.
        public async Task<TicketDetailsDto?> GetAdminDetailsByIdAsync(
            int ticketId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.Id == ticketId)
                .Select(t => new TicketDetailsDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Title = t.Title,
                    Description = t.Description,
                    CategoryName = t.Category.Name,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    ClosedAt = t.ClosedAt,

                    CreatedByName = _context.Users
                        .Where(u => u.Id == t.CreatedById)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault(),

                    CreatedByDepartment = _context.Users
                        .Where(u => u.Id == t.CreatedById)
                        .Select(u => u.Department)
                        .FirstOrDefault(),

                    AssignedTechnicianName = t.AssignedToId == null
                        ? null
                        : _context.Users
                            .Where(u => u.Id == t.AssignedToId)
                            .Select(u => u.FirstName + " " + u.LastName)
                            .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<IReadOnlyList<EmployeeTicketDto>> GetByTechnicianIdAsync(
            string technicianId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.AssignedToId == technicianId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new EmployeeTicketDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Title = t.Title,
                    CategoryName = t.Category.Name,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    AssignedTechnicianName = _context.Users
                        .Where(u => u.Id == t.AssignedToId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<EmployeeTicketDto>> GetAvailableAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.AssignedToId == null && t.Status == "Open")
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new EmployeeTicketDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Title = t.Title,
                    CategoryName = t.Category.Name,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    AssignedTechnicianName = null
                })
                .ToListAsync(cancellationToken);
        }


        public async Task<bool> ClaimTicketAsync(
            int ticketId,
            string technicianId,
            CancellationToken cancellationToken = default)
        {
            var technician = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == technicianId && u.IsActive,
                    cancellationToken);

            if (technician == null)
                return false;

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            var rowsAffected = await _context.Tickets
                .Where(t =>
                    t.Id == ticketId &&
                    t.AssignedToId == null &&
                    t.Status == "Open")
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(t => t.AssignedToId, technicianId)
                        .SetProperty(t => t.UpdatedAt, DateTime.UtcNow),
                    cancellationToken);

            if (rowsAffected == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            var history = new TicketHistory
            {
                TicketId = ticketId,
                UserId = technicianId,
                Action = "Claimed",
                OldValue = null,
                NewValue = technician.FirstName + " " + technician.LastName,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TicketHistories.AddAsync(
                history,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }



        public async Task<bool> AssignToTechnicianAsync(
            int ticketId,
            string technicianId,
            string assignedById,
            CancellationToken cancellationToken = default)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(
                    t => t.Id == ticketId,
                    cancellationToken);

            if (ticket == null)
                return false;

            var technician = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == technicianId && u.IsActive,
                    cancellationToken);

            if (technician == null)
                return false;

            var adminExists = await _context.Users
                .AnyAsync(
                    u => u.Id == assignedById,
                    cancellationToken);

            if (!adminExists)
                return false;

            var previousTechnicianId = ticket.AssignedToId;

            if (previousTechnicianId == technicianId)
                return false;

            string? oldTechnicianName = null;

            if (previousTechnicianId != null)
            {
                oldTechnicianName = await _context.Users
                    .Where(u => u.Id == previousTechnicianId)
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var action = previousTechnicianId == null
                ? "Assigned"
                : "Reassigned";

            var history = new TicketHistory
            {
                TicketId = ticketId,
                UserId = assignedById,
                Action = action,
                OldValue = oldTechnicianName,
                NewValue = technician.FirstName + " " + technician.LastName,
                CreatedAt = DateTime.UtcNow
            };

            ticket.AssignedToId = technicianId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.TicketHistories.AddAsync(
                history,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }



        public async Task<IReadOnlyList<TechnicianConversationDto>> GetTechnicianConversationsAsync(
            string technicianId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.AssignedToId == technicianId)
                .OrderByDescending(t =>
                    _context.TicketComments
                        .Where(c => c.TicketId == t.Id && !c.IsInternal)
                        .Select(c => (DateTime?)c.CreatedAt)
                        .Max() ?? t.CreatedAt)
                .Select(t => new TechnicianConversationDto
                {
                    TicketId = t.Id,
                    TicketNumber = t.TicketNumber,
                    TicketTitle = t.Title,

                    EmployeeName = _context.Users
                        .Where(u => u.Id == t.CreatedById)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "Unknown Employee",

                    LastMessage = _context.TicketComments
                        .Where(c => c.TicketId == t.Id && !c.IsInternal)
                        .OrderByDescending(c => c.CreatedAt)
                        .Select(c => c.Message)
                        .FirstOrDefault(),

                    LastMessageBy = _context.TicketComments
                        .Where(c => c.TicketId == t.Id && !c.IsInternal)
                        .OrderByDescending(c => c.CreatedAt)
                        .Select(c => _context.Users
                            .Where(u => u.Id == c.UserId)
                            .Select(u => u.FirstName + " " + u.LastName)
                            .FirstOrDefault())
                        .FirstOrDefault(),

                    LastMessageAt = _context.TicketComments
                        .Where(c => c.TicketId == t.Id && !c.IsInternal)
                        .Select(c => (DateTime?)c.CreatedAt)
                        .Max(),

                    MessageCount = _context.TicketComments
                        .Count(c => c.TicketId == t.Id && !c.IsInternal),

                    TicketStatus = t.Status
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<TicketDetailsDto?> GetAssignedDetailsByIdAsync(
            int ticketId,
            string technicianId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.Id == ticketId &&
                            t.AssignedToId == technicianId)
                .Select(t => new TicketDetailsDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Title = t.Title,
                    Description = t.Description,
                    CategoryName = t.Category.Name,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    AssignedTechnicianName = _context.Users
                        .Where(u => u.Id == t.AssignedToId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }




        public async Task<bool> UpdateStatusAsync(
            int ticketId,
            string technicianId,
            string newStatus,
            CancellationToken cancellationToken = default)
        {
            var allowedStatuses = new[]
            {
        "Open",
        "In Progress",
        "Resolved"
    };

            if (!allowedStatuses.Contains(newStatus))
                return false;

            var technicianIsActive = await _context.Users
                .AnyAsync(
                    u => u.Id == technicianId && u.IsActive,
                    cancellationToken);

            if (!technicianIsActive)
                return false;

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(
                    t => t.Id == ticketId &&
                         t.AssignedToId == technicianId,
                    cancellationToken);

            if (ticket == null)
                return false;

            var oldStatus = ticket.Status;

            if (oldStatus == newStatus)
                return true;

            ticket.Status = newStatus;
            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.ClosedAt = newStatus == "Resolved"
                ? DateTime.UtcNow
                : null;

            var history = new TicketHistory
            {
                TicketId = ticketId,
                UserId = technicianId,
                Action = "Status Changed",
                OldValue = oldStatus,
                NewValue = newStatus,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TicketHistories.AddAsync(
                history,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }


        public async Task<IReadOnlyList<TicketHistoryDto>> GetHistoryByTicketIdAsync(
            int ticketId,
            CancellationToken cancellationToken = default)
        {
            return await _context.TicketHistories
                .Where(h => h.TicketId == ticketId)
                .OrderByDescending(h => h.CreatedAt)
                .Select(h => new TicketHistoryDto
                {
                    Id = h.Id,
                    Action = h.Action,
                    OldValue = h.OldValue,
                    NewValue = h.NewValue,
                    UserName = _context.Users
                        .Where(u => u.Id == h.UserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "Unknown User",
                    CreatedAt = h.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }


    }
}
