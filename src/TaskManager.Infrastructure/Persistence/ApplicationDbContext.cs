using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<ApplicationUser> UserEntities => Set<ApplicationUser>();
    public DbSet<Workspace> WorkspaceEntities => Set<Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMemberEntities => Set<WorkspaceMember>();
    public DbSet<Project> ProjectEntities => Set<Project>();
    public DbSet<ProjectNote> ProjectNoteEntities => Set<ProjectNote>();
    public DbSet<Board> BoardEntities => Set<Board>();
    public DbSet<BoardColumn> BoardColumnEntities => Set<BoardColumn>();
    public DbSet<TaskItem> TaskItemEntities => Set<TaskItem>();
    public DbSet<Comment> CommentEntities => Set<Comment>();
    public DbSet<Attachment> AttachmentEntities => Set<Attachment>();
    public DbSet<Label> LabelEntities => Set<Label>();
    public DbSet<TaskItemLabel> TaskItemLabelEntities => Set<TaskItemLabel>();
    public DbSet<ChecklistItem> ChecklistItemEntities => Set<ChecklistItem>();

    IQueryable<ApplicationUser> IApplicationDbContext.Users => UserEntities;
    IQueryable<Workspace> IApplicationDbContext.Workspaces => WorkspaceEntities;
    IQueryable<WorkspaceMember> IApplicationDbContext.WorkspaceMembers => WorkspaceMemberEntities;
    IQueryable<Project> IApplicationDbContext.Projects => ProjectEntities;
    IQueryable<ProjectNote> IApplicationDbContext.ProjectNotes => ProjectNoteEntities;
    IQueryable<Board> IApplicationDbContext.Boards => BoardEntities;
    IQueryable<BoardColumn> IApplicationDbContext.BoardColumns => BoardColumnEntities;
    IQueryable<TaskItem> IApplicationDbContext.TaskItems => TaskItemEntities;
    IQueryable<Comment> IApplicationDbContext.Comments => CommentEntities;
    IQueryable<Attachment> IApplicationDbContext.Attachments => AttachmentEntities;
    IQueryable<Label> IApplicationDbContext.Labels => LabelEntities;
    IQueryable<TaskItemLabel> IApplicationDbContext.TaskItemLabels => TaskItemLabelEntities;
    IQueryable<ChecklistItem> IApplicationDbContext.ChecklistItems => ChecklistItemEntities;

    public new void Add<TEntity>(TEntity entity) where TEntity : class => Set<TEntity>().Add(entity);

    public Task<bool> AnyAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default) =>
        EntityFrameworkQueryableExtensions.AnyAsync(query, cancellationToken);

    public Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default) =>
        EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(query, cancellationToken);

    public Task<List<T>> ToListAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default) =>
        EntityFrameworkQueryableExtensions.ToListAsync(query, cancellationToken);

    public Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default) =>
        EntityFrameworkQueryableExtensions.CountAsync(query, cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ApplySoftDeleteFilters(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<Workspace>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<WorkspaceMember>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<Project>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<ProjectNote>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<Board>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<BoardColumn>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<TaskItem>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<Comment>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<Attachment>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<Label>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<TaskItemLabel>().HasQueryFilter(x => x.DeletedAtUtc == null);
        modelBuilder.Entity<ChecklistItem>().HasQueryFilter(x => x.DeletedAtUtc == null);
    }
}
