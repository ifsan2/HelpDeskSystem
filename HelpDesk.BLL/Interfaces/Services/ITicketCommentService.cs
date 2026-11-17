using HelpDesk.BLL.DTOs.Tickets;

namespace HelpDesk.BLL.Interfaces.Services
{
    public interface ITicketCommentService
    {
        Task<IReadOnlyList<TicketCommentDto>> GetByTicketIdAsync(
            int ticketId,
            bool includeInternal,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            int ticketId,
            string userId,
            string message,
            bool isInternal,
            CancellationToken cancellationToken = default);
    }
}