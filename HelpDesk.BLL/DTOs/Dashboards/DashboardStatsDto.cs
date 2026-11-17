
namespace HelpDesk.BLL.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalTickets { get; set; }

        public int OpenTickets { get; set; }

        public int InProgressTickets { get; set; }

        public int ResolvedTickets { get; set; }

        public int TotalEmployees { get; set; }

        public int TotalTechnicians { get; set; }
    }
}
