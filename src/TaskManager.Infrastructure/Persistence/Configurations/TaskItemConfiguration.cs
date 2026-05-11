using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("task_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(10000);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.Priority).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => new { x.ProjectId, x.Status, x.Priority });
        builder.HasIndex(x => new { x.ProjectId, x.BoardColumnId, x.ParentTaskItemId, x.Position });
        builder.HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BoardColumn).WithMany(x => x.TaskItems).HasForeignKey(x => x.BoardColumnId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ParentTaskItem).WithMany(x => x.Subtasks).HasForeignKey(x => x.ParentTaskItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Assignee).WithMany().HasForeignKey(x => x.AssigneeId).OnDelete(DeleteBehavior.SetNull);
    }
}
