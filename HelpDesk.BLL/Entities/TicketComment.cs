namespace HelpDesk.BLL.Entities
{
    public class TicketComment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public Ticket Ticket { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsInternal { get; set; } = false;
    }
}