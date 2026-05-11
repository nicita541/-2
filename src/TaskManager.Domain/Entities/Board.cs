using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class Board : Entity
{
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public required string Name { get; set; }
    public ICollection<BoardColumn> Columns { get; set; } = new List<BoardColumn>();
    public ICollection<Label> Labels { get; set; } = new List<Label>();
}
