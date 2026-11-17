
namespace HelpDesk.BLL.DTOs.Tickets
{
    public class TechnicianConversationDto
    {
        public int TicketId { get; set; }

        public string TicketNumber { get; set; } = string.Empty;

        public string TicketTitle { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = string.Empty;

        public string? LastMessage { get; set; }

        public string? LastMessageBy { get; set; }

        public DateTime? LastMessageAt { get; set; }

        public int MessageCount { get; set; }

        public string TicketStatus { get; set; } = string.Empty;
    }
}
