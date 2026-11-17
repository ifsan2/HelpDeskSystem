
using HelpDesk.BLL.DTOs.Tickets;
using HelpDesk.BLL.Entities;

namespace HelpDesk.BLL.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task AddAsync(
            Ticket ticket,
            CancellationToken cancellationToken = default);

        Task<string?> GetLastTicketNumberAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EmployeeTicketDto>> GetByEmployeeIdAsync(
            string employeeId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EmployeeTicketDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<TicketDetailsDto?> GetDetailsByIdAsync(
            int ticketId,
            string employeeId,
            CancellationToken cancellationToken = default);

        Task<TicketDetailsDto?> GetAdminDetailsByIdAsync(
            int ticketId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EmployeeTicketDto>> GetByTechnicianIdAsync(
            string technicianId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EmployeeTicketDto>> GetAvailableAsync(
            CancellationToken cancellationToken = default);

        Task<bool> ClaimTicketAsync(
            int ticketId,
            string technicianId,
            CancellationToken cancellationToken = default);


        Task<bool> AssignToTechnicianAsync(
            int ticketId,
            string technicianId,
            string assignedById,
            CancellationToken cancellationToken = default);


        Task<IReadOnlyList<TechnicianConversationDto>> GetTechnicianConversationsAsync(
            string technicianId,
            CancellationToken cancellationToken = default);

        Task<TicketDetailsDto?> GetAssignedDetailsByIdAsync(
            int ticketId,
            string technicianId,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateStatusAsync(
            int ticketId,
            string technicianId,
            string newStatus,
            CancellationToken cancellationToken = default);


        Task<IReadOnlyList<TicketHistoryDto>> GetHistoryByTicketIdAsync(
            int ticketId,
            CancellationToken cancellationToken = default);

    }
}
