namespace TaskManager.Application.ProjectNotes.Dtos;

public sealed record ProjectNoteCreateRequest(string Title, string ContentMarkdown);
public sealed record ProjectNoteResponse(Guid Id, Guid ProjectId, string Title, string ContentMarkdown, DateTime CreatedAtUtc);
