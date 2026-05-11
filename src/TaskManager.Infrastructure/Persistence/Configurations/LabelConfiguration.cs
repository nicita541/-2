using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("labels");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ColorHex).HasMaxLength(7).IsRequired();
        builder.HasIndex(x => new { x.BoardId, x.Name }).IsUnique();
        builder.HasOne(x => x.Board).WithMany(x => x.Labels).HasForeignKey(x => x.BoardId).OnDelete(DeleteBehavior.Cascade);
    }
}
