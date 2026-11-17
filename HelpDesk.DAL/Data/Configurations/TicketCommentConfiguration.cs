using HelpDesk.BLL.Entities;
using HelpDesk.DAL.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.DAL.Data.Configurations
{
    public class TicketCommentConfiguration
        : IEntityTypeConfiguration<TicketComment>
    {
        public void Configure(EntityTypeBuilder<TicketComment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(c => c.Message)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.IsInternal)
                .IsRequired();

            builder.HasOne(c => c.Ticket)
                .WithMany()
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.TicketId);

            builder.HasIndex(c => c.UserId);

            builder.HasIndex(c => c.CreatedAt);
        }
    }
}