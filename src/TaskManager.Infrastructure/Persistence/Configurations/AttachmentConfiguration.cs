using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("attachments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileName).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Url).HasMaxLength(2000).IsRequired();
        builder.HasOne(x => x.TaskItem).WithMany(x => x.Attachments).HasForeignKey(x => x.TaskItemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.UploadedBy).WithMany(x => x.Attachments).HasForeignKey(x => x.UploadedById).OnDelete(DeleteBehavior.Restrict);
    }
}
