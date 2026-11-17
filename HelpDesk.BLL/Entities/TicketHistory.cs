namespace HelpDesk.BLL.Entities
{
    public class TicketHistory
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public Ticket Ticket { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}