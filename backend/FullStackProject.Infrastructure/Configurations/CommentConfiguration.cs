using FullStackProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FullStackProject.Infrastructure.Configurations;

public class CommentConfiguration
    : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.TaskItem)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TaskItemId);
    }
}
