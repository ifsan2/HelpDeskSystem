
using HelpDesk.BLL.DTOs.Dashboard;

namespace HelpDesk.BLL.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsAsync(
            CancellationToken cancellationToken = default);


        Task<IReadOnlyList<TicketActivityDto>> GetTicketActivityAsync(
            int days = 7,
            CancellationToken cancellationToken = default);

    }
}
