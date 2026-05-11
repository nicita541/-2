using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class TaskItemLabelConfiguration : IEntityTypeConfiguration<TaskItemLabel>
{
    public void Configure(EntityTypeBuilder<TaskItemLabel> builder)
    {
        builder.ToTable("task_item_labels");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.TaskItemId, x.LabelId }).IsUnique();
        builder.HasOne(x => x.TaskItem).WithMany(x => x.Labels).HasForeignKey(x => x.TaskItemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Label).WithMany(x => x.TaskItems).HasForeignKey(x => x.LabelId).OnDelete(DeleteBehavior.Cascade);
    }
}
