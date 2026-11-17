namespace HelpDesk.BLL.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        public string TicketNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public string CreatedById { get; set; } = string.Empty;

        public string? AssignedToId { get; set; }

        public string Priority { get; set; } = "Medium";

        public string Status { get; set; } = "Open";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}