
using HelpDesk.BLL.DTOs.Tickets;
using HelpDesk.BLL.Entities;
using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.BLL.Interfaces.Services;

namespace HelpDesk.BLL.Services
{
    public class TicketCommentService : ITicketCommentService
    {
        private const int MaxCommentLength = 2000;

        private readonly ITicketCommentRepository _ticketCommentRepository;

        public TicketCommentService(
            ITicketCommentRepository ticketCommentRepository)
        {
            _ticketCommentRepository = ticketCommentRepository;
        }

        public async Task<IReadOnlyList<TicketCommentDto>> GetByTicketIdAsync(
            int ticketId,
            bool includeInternal,
            CancellationToken cancellationToken = default)
        {
            if (ticketId <= 0)
            {
                throw new ArgumentException("Invalid ticket ID.");
            }

            return await _ticketCommentRepository.GetByTicketIdAsync(
                ticketId,
                includeInternal,
                cancellationToken);
        }

        public async Task AddAsync(
            int ticketId,
            string userId,
            string message,
            bool isInternal,
            CancellationToken cancellationToken = default)
        {
            if (ticketId <= 0)
            {
                throw new ArgumentException("Invalid ticket ID.");
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID is required.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Comment message is required.");
            }

            string trimmedMessage = message.Trim();

            if (trimmedMessage.Length > MaxCommentLength)
            {
                throw new ArgumentException(
                    "Comment cannot exceed 2000 characters.");
            }

            var comment = new TicketComment
            {
                TicketId = ticketId,
                UserId = userId,
                Message = trimmedMessage,
                IsInternal = isInternal,
                CreatedAt = DateTime.UtcNow
            };

            await _ticketCommentRepository.AddAsync(
                comment,
                cancellationToken);
        }
    }
}
