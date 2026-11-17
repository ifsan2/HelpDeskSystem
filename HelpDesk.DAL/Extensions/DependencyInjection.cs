using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.DAL.Repositories;
using HelpDesk.DAL.Data;
using HelpDesk.DAL.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace HelpDesk.DAL.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database registration
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // ASP.NET Core Identity registration
            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            // Repository registration
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<ITicketRepository, TicketRepository>();

            services.AddScoped<ITicketCommentRepository, TicketCommentRepository>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IDashboardRepository, DashboardRepository>();

            return services;
        }
    }
}