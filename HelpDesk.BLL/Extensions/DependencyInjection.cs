
using HelpDesk.BLL.Interfaces.Services;
using HelpDesk.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.BLL.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogic(
            this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<ITicketService, TicketService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }
    }
}
