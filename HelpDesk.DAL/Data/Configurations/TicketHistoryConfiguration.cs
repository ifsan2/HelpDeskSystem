using HelpDesk.BLL.Entities;
using HelpDesk.DAL.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.DAL.Data.Configurations
{
    public class TicketHistoryConfiguration
        : IEntityTypeConfiguration<TicketHistory>
    {
        public void Configure(EntityTypeBuilder<TicketHistory> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(h => h.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.OldValue)
                .HasMaxLength(1000);

            builder.Property(h => h.NewValue)
                .HasMaxLength(1000);

            builder.Property(h => h.CreatedAt)
                .IsRequired();

            builder.HasOne(h => h.Ticket)
                .WithMany()
                .HasForeignKey(h => h.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(h => h.TicketId);

            builder.HasIndex(h => h.UserId);

            builder.HasIndex(h => h.CreatedAt);
        }
    }
}