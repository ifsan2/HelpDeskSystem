using HelpDesk.BLL.Entities;
using HelpDesk.DAL.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.DAL.Data.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.TicketNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(t => t.TicketNumber)
                .IsUnique();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(t => t.Priority)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee who created the ticket
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Technician assigned to the ticket
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(t => t.CreatedById);

            builder.HasIndex(t => t.AssignedToId);

            builder.HasIndex(t => t.Status);

            builder.HasIndex(t => t.Priority);

            builder.HasIndex(t => t.CreatedAt);
        }
    }
}