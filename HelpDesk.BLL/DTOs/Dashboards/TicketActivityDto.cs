
namespace HelpDesk.BLL.DTOs.Dashboard
{
    public class TicketActivityDto
    {
        public DateTime Date { get; set; }

        public int CreatedTickets { get; set; }

        public int ResolvedTickets { get; set; }
    }
}
