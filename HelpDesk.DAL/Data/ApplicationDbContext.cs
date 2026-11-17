using HelpDesk.BLL.Entities;
using HelpDesk.DAL.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<TicketComment> TicketComments { get; set; }

        public DbSet<TicketHistory> TicketHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}