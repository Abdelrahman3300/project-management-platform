using FullStackProject.Domain.Entities;
using FullStackProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FullStackProject.Infrastructure.Configurations;

public class TaskItemConfiguration
    : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Project)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AssignedToUser)
            .WithMany(x => x.AssignedTasks)
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.ProjectId);

        builder.HasIndex(x => x.AssignedToUserId);
    }
}
