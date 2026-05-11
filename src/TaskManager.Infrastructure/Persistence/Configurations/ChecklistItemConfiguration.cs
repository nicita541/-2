using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
{
    public void Configure(EntityTypeBuilder<ChecklistItem> builder)
    {
        builder.ToTable("checklist_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Text).HasMaxLength(1000).IsRequired();
        builder.HasIndex(x => new { x.TaskItemId, x.Position });
        builder.HasOne(x => x.TaskItem).WithMany(x => x.Checklist).HasForeignKey(x => x.TaskItemId).OnDelete(DeleteBehavior.Cascade);
    }
}
