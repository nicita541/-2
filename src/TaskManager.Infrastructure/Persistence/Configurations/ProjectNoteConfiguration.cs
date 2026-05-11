using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class ProjectNoteConfiguration : IEntityTypeConfiguration<ProjectNote>
{
    public void Configure(EntityTypeBuilder<ProjectNote> builder)
    {
        builder.ToTable("project_notes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
        builder.Property(x => x.ContentMarkdown).HasMaxLength(20000).IsRequired();
        builder.HasIndex(x => x.ProjectId);
        builder.HasOne(x => x.Project).WithMany(x => x.Notes).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}
