
using HelpDesk.BLL.DTOs.Dashboard;

namespace HelpDesk.BLL.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<DashboardStatsDto> GetStatsAsync(
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TicketActivityDto>> GetTicketActivityAsync(
            int days = 7,
            CancellationToken cancellationToken = default);
    }
}
