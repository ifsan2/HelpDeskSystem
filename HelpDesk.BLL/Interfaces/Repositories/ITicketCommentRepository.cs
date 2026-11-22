
using HelpDesk.BLL.DTOs.Tickets;
using HelpDesk.BLL.Entities;

namespace HelpDesk.BLL.Interfaces.Repositories
{
    public interface ITicketCommentRepository
    {
        Task<IReadOnlyList<TicketCommentDto>> GetByTicketIdAsync(
            int ticketId,
            string requestingUserId,
            bool includeInternal,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            TicketComment comment,
            CancellationToken cancellationToken = default);
    }
}
