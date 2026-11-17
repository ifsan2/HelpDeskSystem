
namespace HelpDesk.BLL.DTOs.Tickets
{
    public class TicketDetailsDto
    {
        public int Id { get; set; }

        public string TicketNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public string? CreatedByName { get; set; }

        public string? CreatedByDepartment { get; set; }

        public string? AssignedTechnicianName { get; set; }
    }
}
