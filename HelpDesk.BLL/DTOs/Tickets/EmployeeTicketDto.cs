
namespace HelpDesk.BLL.DTOs.Tickets
{
    public class EmployeeTicketDto
    {
        public int Id { get; set; }

        public string TicketNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string? AssignedTechnicianId { get; set; }

        public string? AssignedTechnicianName { get; set; }
    }
}
