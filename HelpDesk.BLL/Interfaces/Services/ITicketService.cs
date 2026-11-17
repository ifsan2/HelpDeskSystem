
using HelpDesk.BLL.DTOs.Tickets;

namespace HelpDesk.BLL.Interfaces.Services
{
    public interface ITicketService
    {
        Task<string> CreateAsync(
            CreateTicketDto ticket,
            string createdById,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EmployeeTicketDto>> GetMyTicketsAsync(
            string employeeId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EmployeeTicketDto>> GetAllTicketsAsync(
            CancellationToken cancellationToken = default);

        Task<TicketDetailsDto?> GetDetailsAsync(
            int ticketId,
            string employeeId,
            CancellationToken cancellationToken = default);

        Task<TicketDetailsDto?> GetAdminDetailsAsync(
            int ticketId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EmployeeTicketDto>> GetMyAssignedTicketsAsync(
            string technicianId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<EmployeeTicketDto>> GetAvailableTicketsAsync(
            CancellationToken cancellationToken = default);

        Task<bool> ClaimTicketAsync(
            int ticketId,
            string technicianId,
            CancellationToken cancellationToken = default);


        Task<bool> AssignTicketAsync(
            int ticketId,
            string technicianId,
            string assignedById,
            CancellationToken cancellationToken = default);


        Task<IReadOnlyList<TechnicianConversationDto>> GetTechnicianConversationsAsync(
            string technicianId,
            CancellationToken cancellationToken = default);

        Task<TicketDetailsDto?> GetAssignedDetailsAsync(
            int ticketId,
            string technicianId,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateStatusAsync(
            int ticketId,
            string technicianId,
            string newStatus,
            CancellationToken cancellationToken = default);


        Task<IReadOnlyList<TicketHistoryDto>> GetHistoryAsync(
            int ticketId,
            CancellationToken cancellationToken = default);

    }
}
