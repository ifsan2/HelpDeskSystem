
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
            if (ticket == null ||
                string.IsNullOrWhiteSpace(ticket.CreatedById))
            {
                throw new ArgumentException("Invalid ticket or employee ID.");
            }

            // Verify that the employee exists, is active,
            // and has the actual Employee role.
            var employeeIsValid = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == ticket.CreatedById
                      && user.IsActive
                      && role.Name == "Employee"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!employeeIsValid)
            {
                throw new UnauthorizedAccessException(
                    "Only active employees can create tickets.");
            }

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


        public async Task<int> GetNextTicketNumberAsync(
            CancellationToken cancellationToken = default)
        {
            var connection = _context.Database.GetDbConnection();

            bool shouldCloseConnection =
                connection.State != System.Data.ConnectionState.Open;

            if (shouldCloseConnection)
            {
                await connection.OpenAsync(cancellationToken);
            }

            try
            {
                await using var command = connection.CreateCommand();

                command.CommandText =
                    "SELECT NEXT VALUE FOR dbo.TicketNumberSequence;";

                var result = await command.ExecuteScalarAsync(cancellationToken);

                return Convert.ToInt32(result);
            }
            finally
            {
                if (shouldCloseConnection)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<IReadOnlyList<EmployeeTicketDto>> GetByEmployeeIdAsync(
            string employeeId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return Array.Empty<EmployeeTicketDto>();
            }

            var isActiveEmployee = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == employeeId
                      && user.IsActive
                      && role.Name == "Employee"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!isActiveEmployee)
            {
                return Array.Empty<EmployeeTicketDto>();
            }

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
            string adminId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(adminId))
            {
                return Array.Empty<EmployeeTicketDto>();
            }

            var isActiveAdmin = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == adminId
                      && user.IsActive
                      && role.Name == "Admin"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!isActiveAdmin)
            {
                return Array.Empty<EmployeeTicketDto>();
            }

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
            if (ticketId <= 0 || string.IsNullOrWhiteSpace(employeeId))
            {
                return null;
            }

            var isActiveEmployee = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == employeeId
                      && user.IsActive
                      && role.Name == "Employee"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!isActiveEmployee)
            {
                return null;
            }

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
            string adminId,
            CancellationToken cancellationToken = default)
        {
            if (ticketId <= 0 || string.IsNullOrWhiteSpace(adminId))
            {
                return null;
            }

            // Verify that the Admin account exists, is active,
            // and has the actual Admin role.
            var adminIsValid = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == adminId
                      && user.IsActive
                      && role.Name == "Admin"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!adminIsValid)
            {
                return null;
            }

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
            if (string.IsNullOrWhiteSpace(technicianId))
            {
                return Array.Empty<EmployeeTicketDto>();
            }

            var isActiveTechnician = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == technicianId
                      && user.IsActive
                      && role.Name == "Technician"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!isActiveTechnician)
            {
                return Array.Empty<EmployeeTicketDto>();
            }

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
           string technicianId,
           CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(technicianId))
            {
                return Array.Empty<EmployeeTicketDto>();
            }

            // Verify that the user exists, is active, and has the actual Technician role.
            var technicianIsValid = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == technicianId
                      && user.IsActive
                      && role.Name == "Technician"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!technicianIsValid)
            {
                return Array.Empty<EmployeeTicketDto>();
            }

            return await _context.Tickets
                .AsNoTracking()
                .Where(t =>
                    t.AssignedToId == null &&
                    t.Status == "Open")
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
            if (ticketId <= 0 || string.IsNullOrWhiteSpace(technicianId))
            {
                return false;
            }

            // Verify that the user exists and is active.
            var technician = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == technicianId && u.IsActive,
                    cancellationToken);

            if (technician == null)
            {
                return false;
            }

            // Verify the user's actual Technician role.
            var hasTechnicianRole = await (
                from userRole in _context.UserRoles
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where userRole.UserId == technicianId
                      && role.Name == "Technician"
                select userRole
            ).AnyAsync(cancellationToken);

            if (!hasTechnicianRole)
            {
                return false;
            }

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
            if (ticketId <= 0 ||
                string.IsNullOrWhiteSpace(technicianId) ||
                string.IsNullOrWhiteSpace(assignedById))
            {
                return false;
            }

            // Verify that the assigning user is an active Admin.
            var adminIsValid = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == assignedById
                      && user.IsActive
                      && role.Name == "Admin"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!adminIsValid)
                return false;

            // Verify that the target user is an active Technician.
            var technicianIsValid = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == technicianId
                      && user.IsActive
                      && role.Name == "Technician"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!technicianIsValid)
                return false;

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(
                    t => t.Id == ticketId,
                    cancellationToken);

            if (ticket == null)
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

            var technicianName = await _context.Users
                .Where(u => u.Id == technicianId)
                .Select(u => u.FirstName + " " + u.LastName)
                .FirstOrDefaultAsync(cancellationToken);

            var history = new TicketHistory
            {
                TicketId = ticketId,
                UserId = assignedById,
                Action = previousTechnicianId == null
                    ? "Assigned"
                    : "Reassigned",
                OldValue = oldTechnicianName,
                NewValue = technicianName,
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
            if (string.IsNullOrWhiteSpace(technicianId))
            {
                return Array.Empty<TechnicianConversationDto>();
            }

            // Verify that the user exists, is active, and has the actual Technician role.
            var technicianIsValid = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == technicianId
                      && user.IsActive
                      && role.Name == "Technician"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!technicianIsValid)
            {
                return Array.Empty<TechnicianConversationDto>();
            }

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
            if (ticketId <= 0 || string.IsNullOrWhiteSpace(technicianId))
            {
                return null;
            }

            var isActiveTechnician = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == technicianId
                      && user.IsActive
                      && role.Name == "Technician"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!isActiveTechnician)
            {
                return null;
            }

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
            if (ticketId <= 0 ||
                string.IsNullOrWhiteSpace(technicianId) ||
                string.IsNullOrWhiteSpace(newStatus))
            {
                return false;
            }

            var allowedStatuses = new[]
            {
        "Open",
        "In Progress",
        "Resolved"
    };

            if (!allowedStatuses.Contains(newStatus))
                return false;

            // Verify that the user is an active Technician.
            var technicianIsValid = await (
                from user in _context.Users
                join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where user.Id == technicianId
                      && user.IsActive
                      && role.Name == "Technician"
                select user.Id
            ).AnyAsync(cancellationToken);

            if (!technicianIsValid)
                return false;

            // Only the assigned Technician can update the ticket.
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
            string requestingUserId,
            CancellationToken cancellationToken = default)
        {
            if (ticketId <= 0 || string.IsNullOrWhiteSpace(requestingUserId))
            {
                throw new ArgumentException("Invalid ticket or requesting user ID.");
            }

            // Verify that the requesting user exists and is active.
            var userIsActive = await _context.Users
                .AnyAsync(
                    u => u.Id == requestingUserId && u.IsActive,
                    cancellationToken);

            if (!userIsActive)
            {
                throw new UnauthorizedAccessException(
                    "Your account is not authorized to access ticket history.");
            }

            // Check whether the requesting user has the Admin role.
            var isAdmin = await (
                from userRole in _context.UserRoles
                join role in _context.Roles
                    on userRole.RoleId equals role.Id
                where userRole.UserId == requestingUserId
                      && role.Name == "Admin"
                select userRole
            ).AnyAsync(cancellationToken);

            // Find the ticket and its ownership/assignment information.
            var ticket = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.Id == ticketId)
                .Select(t => new
                {
                    t.CreatedById,
                    t.AssignedToId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (ticket == null)
            {
                return Array.Empty<TicketHistoryDto>();
            }

            // Admins, ticket creators, and assigned technicians can view history.
            var hasAccess =
                isAdmin ||
                ticket.CreatedById == requestingUserId ||
                ticket.AssignedToId == requestingUserId;

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to access this ticket history.");
            }

            return await _context.TicketHistories
                .AsNoTracking()
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
