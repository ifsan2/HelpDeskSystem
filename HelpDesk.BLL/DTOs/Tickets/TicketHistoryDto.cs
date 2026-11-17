
namespace HelpDesk.BLL.DTOs.Tickets
{
    public class TicketHistoryDto
    {
        public int Id { get; set; }

        public string Action { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string UserName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
