
using HelpDesk.BLL.DTOs.Dashboard;
using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.BLL.Interfaces.Services;

namespace HelpDesk.BLL.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardStatsDto> GetStatsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dashboardRepository.GetStatsAsync(cancellationToken);
        }


        public async Task<IReadOnlyList<TicketActivityDto>> GetTicketActivityAsync(
            int days = 7,
            CancellationToken cancellationToken = default)
        {
            return await _dashboardRepository.GetTicketActivityAsync(
                days,
                cancellationToken);
        }

    }
}
