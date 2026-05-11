namespace TaskManager.Application.Comments.Dtos;

public sealed record CommentCreateRequest(Guid TaskItemId, string Body);
public sealed record CommentResponse(Guid Id, Guid TaskItemId, Guid AuthorId, string Body, DateTime CreatedAtUtc);
