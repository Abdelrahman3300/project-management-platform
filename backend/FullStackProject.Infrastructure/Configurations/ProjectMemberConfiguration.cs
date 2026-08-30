
using FullStackProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FullStackProject.Infrastructure.Configurations;

public class ProjectMemberConfiguration
    : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.HasKey(x => new
        {
            x.ProjectId,
            x.UserId
        });

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.JoinedAt)
            .IsRequired();

        builder.HasOne(x => x.Project)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.ProjectMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
