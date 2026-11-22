
using HelpDesk.BLL.DTOs.Tickets;
using HelpDesk.BLL.Entities;
using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.BLL.Interfaces.Services;

namespace HelpDesk.BLL.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TicketService(
            ITicketRepository ticketRepository,
            ICategoryRepository categoryRepository)
        {
            _ticketRepository = ticketRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<string> CreateAsync(
            CreateTicketDto ticket,
            string createdById,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ticket.Title))
            {
                throw new ArgumentException(
                    "Ticket title is required.");
            }

            if (ticket.Title.Trim().Length > 100)
            {
                throw new ArgumentException(
                    "Ticket title cannot exceed 100 characters.");
            }

            if (string.IsNullOrWhiteSpace(ticket.Description))
            {
                throw new ArgumentException(
                    "Ticket description is required.");
            }

            int descriptionWordCount = ticket.Description
                .Split(
                    (char[]?)null,
                    StringSplitOptions.RemoveEmptyEntries)
                .Length;

            if (descriptionWordCount > 2000)
            {
                throw new ArgumentException(
                    "Ticket description cannot exceed 2000 words.");
            }

            if (string.IsNullOrWhiteSpace(createdById))
            {
                throw new ArgumentException(
                    "Ticket creator is required.");
            }

            var category = await _categoryRepository.GetByIdAsync(
                ticket.CategoryId,
                cancellationToken);

            if (category is null || !category.IsActive)
            {
                throw new InvalidOperationException(
                    "Please select a valid active category.");
            }

            var allowedPriorities = new[]
            {
                "Low",
                "Medium",
                "High",
                "Critical"
            };

            if (!allowedPriorities.Contains(ticket.Priority))
            {
                throw new ArgumentException(
                    "Please select a valid ticket priority.");
            }

            int nextNumber =
                await _ticketRepository.GetNextTicketNumberAsync(
                    cancellationToken);

            string ticketNumber = $"HD-{nextNumber:D6}";

            var newTicket = new Ticket
            {
                TicketNumber = ticketNumber,
                Title = ticket.Title.Trim(),
                Description = ticket.Description.Trim(),
                CategoryId = ticket.CategoryId,
                CreatedById = createdById,
                Priority = ticket.Priority,
                Status = "Open",
                CreatedAt = DateTime.UtcNow
            };

            await _ticketRepository.AddAsync(
                newTicket,
                cancellationToken);

            return ticketNumber;
        }

        public async Task<IReadOnlyList<EmployeeTicketDto>> GetMyTicketsAsync(
            string employeeId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                throw new ArgumentException("Employee ID is required.");
            }

            return await _ticketRepository.GetByEmployeeIdAsync(
                employeeId,
                cancellationToken);
        }

        public async Task<IReadOnlyList<EmployeeTicketDto>> GetAllTicketsAsync(
            string adminId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(adminId))
            {
                return Array.Empty<EmployeeTicketDto>();
            }

            return await _ticketRepository.GetAllAsync(
                adminId,
                cancellationToken);
        }

        public async Task<TicketDetailsDto?> GetDetailsAsync(
            int ticketId,
            string employeeId,
            CancellationToken cancellationToken = default)
        {
            if (ticketId <= 0)
            {
                throw new ArgumentException("Invalid ticket ID.");
            }

            if (string.IsNullOrWhiteSpace(employeeId))
            {
                throw new ArgumentException("Employee ID is required.");
            }

            return await _ticketRepository.GetDetailsByIdAsync(
                ticketId,
                employeeId,
                cancellationToken);
        }

        public async Task<TicketDetailsDto?> GetAdminDetailsAsync(
            int ticketId,
            string adminId,
            CancellationToken cancellationToken = default)
        {
            if (ticketId <= 0)
            {
                throw new ArgumentException("Invalid ticket ID.");
            }

            if (string.IsNullOrWhiteSpace(adminId))
            {
                throw new ArgumentException("Invalid admin ID.");
            }

            return await _ticketRepository.GetAdminDetailsByIdAsync(
                ticketId,
                adminId,
                cancellationToken);
        }

        public async Task<IReadOnlyList<EmployeeTicketDto>> GetMyAssignedTicketsAsync(
            string technicianId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(technicianId))
            {
                throw new ArgumentException(
                    "Technician ID is required.",
                    nameof(technicianId));
            }

            return await _ticketRepository.GetByTechnicianIdAsync(
                technicianId,
                cancellationToken);
        }

        public async Task<IReadOnlyList<EmployeeTicketDto>> GetAvailableTicketsAsync(
            string technicianId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(technicianId))
            {
                throw new ArgumentException(
                    "Technician ID is required.",
                    nameof(technicianId));
            }

            return await _ticketRepository.GetAvailableAsync(
                technicianId,
                cancellationToken);
        }

        public async Task<bool> ClaimTicketAsync(
            int ticketId,
            string technicianId,
            CancellationToken cancellationToken = default)
        {
            if (ticketId <= 0 ||
                string.IsNullOrWhiteSpace(technicianId))
            {
                return false;
            }

            return await _ticketRepository.ClaimTicketAsync(
                ticketId,
                technicianId,
                cancellationToken);
        }

        public async Task<bool> AssignTicketAsync(
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

            return await _ticketRepository.AssignToTechnicianAsync(
                ticketId,
                technicianId,
                assignedById,
                cancellationToken);
        }

        public async Task<IReadOnlyList<TechnicianConversationDto>> GetTechnicianConversationsAsync(
            string technicianId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(technicianId))
            {
                throw new ArgumentException(
                    "Technician ID is required.",
                    nameof(technicianId));
            }

            return await _ticketRepository.GetTechnicianConversationsAsync(
                technicianId,
                cancellationToken);
        }

        public async Task<TicketDetailsDto?> GetAssignedDetailsAsync(
            int ticketId,
            string technicianId,
            CancellationToken cancellationToken = default)
        {
            if (ticketId <= 0 || string.IsNullOrWhiteSpace(technicianId))
            {
                return null;
            }

            return await _ticketRepository.GetAssignedDetailsByIdAsync(
                ticketId,
                technicianId,
                cancellationToken);
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

            return await _ticketRepository.UpdateStatusAsync(
                ticketId,
                technicianId,
                newStatus,
                cancellationToken);
        }

        public async Task<IReadOnlyList<TicketHistoryDto>> GetHistoryAsync(
            int ticketId,
            string requestingUserId,
            CancellationToken cancellationToken = default)
        {
            if (ticketId <= 0)
            {
                throw new ArgumentException("Invalid ticket ID.");
            }

            if (string.IsNullOrWhiteSpace(requestingUserId))
            {
                throw new ArgumentException(
                    "Requesting user ID is required.");
            }

            return await _ticketRepository.GetHistoryByTicketIdAsync(
                ticketId,
                requestingUserId,
                cancellationToken);
        }
    }
}
