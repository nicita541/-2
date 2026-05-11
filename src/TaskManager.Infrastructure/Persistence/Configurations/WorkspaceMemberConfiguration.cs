using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("workspace_members");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.WorkspaceId, x.UserId }).IsUnique();
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.HasOne(x => x.Workspace).WithMany(x => x.Members).HasForeignKey(x => x.WorkspaceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.User).WithMany(x => x.WorkspaceMemberships).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
