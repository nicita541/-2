using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TaskManager.Api.IntegrationTests;

public sealed class ApiIntegrationTests : IClassFixture<TaskManagerApiFactory>
{
    private readonly TaskManagerApiFactory _factory;

    public ApiIntegrationTests(TaskManagerApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_Login_And_Create_Task_Flow_Works()
    {
        using var client = _factory.CreateClient();

        var auth = await RegisterAsync(client, "owner@example.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var workspace = await PostAsync<EntityResponse>(client, "/api/workspaces", new { name = "Workspace", description = "Main" });
        var project = await PostAsync<EntityResponse>(client, "/api/projects", new { workspaceId = workspace.Id, name = "Project", description = "Build" });
        var board = await PostAsync<EntityResponse>(client, "/api/boards", new { projectId = project.Id, name = "Board" });
        var column = await PostAsync<EntityResponse>(client, "/api/columns", new { boardId = board.Id, name = "Todo", position = 0 });
        var task = await PostAsync<TaskItemResponse>(client, "/api/taskitems", new
        {
            projectId = project.Id,
            boardColumnId = column.Id,
            parentTaskItemId = (Guid?)null,
            title = "First task",
            description = "Body",
            position = 0,
            dueDateUtc = (DateTime?)null,
            assigneeId = auth.UserId
        });

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal(project.Id, task.ProjectId);

        var overview = await client.GetFromJsonAsync<ProjectOverviewResponse>($"/api/projects/{project.Id}/overview");
        Assert.Equal(project.Id, overview!.Id);

        var kanban = await client.GetFromJsonAsync<KanbanResponse>($"/api/boards/{board.Id}/kanban");
        Assert.Single(kanban!.Columns);
        Assert.Single(kanban.Columns[0].Tasks);

        var details = await client.GetFromJsonAsync<TaskDetailsResponse>($"/api/taskitems/{task.Id}/details");
        Assert.Equal(task.Id, details!.Id);

        var moved = await PostAsync<TaskItemResponse>(client, $"/api/taskitems/{task.Id}/move", new
        {
            targetBoardColumnId = column.Id,
            targetParentTaskItemId = (Guid?)null,
            newOrder = 2
        });
        Assert.Equal(task.Id, moved.Id);

        var updated = await PutAsync<TaskItemResponse>(client, $"/api/taskitems/{task.Id}", new
        {
            projectId = project.Id,
            boardColumnId = column.Id,
            parentTaskItemId = (Guid?)null,
            title = "Updated task",
            description = "Updated",
            position = 2,
            dueDateUtc = (DateTime?)null,
            assigneeId = (Guid?)null
        });
        Assert.Equal(task.Id, updated.Id);

        var subtask = await PostAsync<TaskItemResponse>(client, $"/api/taskitems/{task.Id}/subtasks", new
        {
            projectId = project.Id,
            boardColumnId = column.Id,
            parentTaskItemId = (Guid?)null,
            title = "Subtask",
            description = "",
            position = 0,
            dueDateUtc = (DateTime?)null,
            assigneeId = (Guid?)null
        });
        Assert.Equal(project.Id, subtask.ProjectId);
    }

    [Fact]
    public async Task User_Cannot_Access_Foreign_Workspace_Project_Or_Task()
    {
        using var ownerClient = _factory.CreateClient();
        var owner = await RegisterAsync(ownerClient, "owner2@example.com");
        ownerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", owner.AccessToken);

        var workspace = await PostAsync<EntityResponse>(ownerClient, "/api/workspaces", new { name = "Private", description = "" });
        var project = await PostAsync<EntityResponse>(ownerClient, "/api/projects", new { workspaceId = workspace.Id, name = "Secret", description = "" });
        var board = await PostAsync<EntityResponse>(ownerClient, "/api/boards", new { projectId = project.Id, name = "Board" });
        var column = await PostAsync<EntityResponse>(ownerClient, "/api/columns", new { boardId = board.Id, name = "Todo", position = 0 });
        var task = await PostAsync<TaskItemResponse>(ownerClient, "/api/taskitems", new
        {
            projectId = project.Id,
            boardColumnId = column.Id,
            parentTaskItemId = (Guid?)null,
            title = "Secret task",
            description = "",
            position = 0,
            dueDateUtc = (DateTime?)null,
            assigneeId = (Guid?)null
        });

        using var outsiderClient = _factory.CreateClient();
        var outsider = await RegisterAsync(outsiderClient, "outsider@example.com");
        outsiderClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", outsider.AccessToken);

        Assert.Equal(HttpStatusCode.Forbidden, (await outsiderClient.GetAsync($"/api/projects?workspaceId={workspace.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await outsiderClient.GetAsync($"/api/boards?projectId={project.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await outsiderClient.GetAsync($"/api/taskitems/{task.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await outsiderClient.GetAsync($"/api/boards/{board.Id}/kanban")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await outsiderClient.GetAsync($"/api/taskitems/{task.Id}/details")).StatusCode);

        var createInForeignColumn = await outsiderClient.PostAsJsonAsync("/api/taskitems", new
        {
            projectId = project.Id,
            boardColumnId = column.Id,
            parentTaskItemId = (Guid?)null,
            title = "Not allowed",
            description = "",
            position = 0,
            dueDateUtc = (DateTime?)null,
            assigneeId = (Guid?)null
        });
        Assert.Equal(HttpStatusCode.Forbidden, createInForeignColumn.StatusCode);

        var moveInForeignColumn = await outsiderClient.PostAsJsonAsync($"/api/taskitems/{task.Id}/move", new
        {
            targetBoardColumnId = column.Id,
            targetParentTaskItemId = (Guid?)null,
            newOrder = 1
        });
        Assert.Equal(HttpStatusCode.Forbidden, moveInForeignColumn.StatusCode);
    }

    [Fact]
    public async Task Deleted_TaskItem_Is_Not_Returned_By_List_Endpoint()
    {
        using var client = _factory.CreateClient();
        var auth = await RegisterAsync(client, "deleter@example.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var workspace = await PostAsync<EntityResponse>(client, "/api/workspaces", new { name = "Workspace", description = "" });
        var project = await PostAsync<EntityResponse>(client, "/api/projects", new { workspaceId = workspace.Id, name = "Project", description = "" });
        var board = await PostAsync<EntityResponse>(client, "/api/boards", new { projectId = project.Id, name = "Board" });
        var column = await PostAsync<EntityResponse>(client, "/api/columns", new { boardId = board.Id, name = "Todo", position = 0 });
        var task = await PostAsync<TaskItemResponse>(client, "/api/taskitems", new
        {
            projectId = project.Id,
            boardColumnId = column.Id,
            parentTaskItemId = (Guid?)null,
            title = "Disposable",
            description = "",
            position = 0,
            dueDateUtc = (DateTime?)null,
            assigneeId = (Guid?)null
        });

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/taskitems/{task.Id}")).StatusCode);

        var list = await client.GetFromJsonAsync<PagedResponse<TaskItemResponse>>($"/api/taskitems?boardColumnId={column.Id}");
        Assert.NotNull(list);
        Assert.Empty(list.Items);
    }

    [Fact]
    public async Task Synced_Metadata_Fields_And_Task_Details_Collections_Are_Persisted()
    {
        using var client = _factory.CreateClient();
        var auth = await RegisterAsync(client, "sync@example.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var workspace = await PostAsync<WorkspaceResponse>(client, "/api/workspaces", new { name = "Team ws", description = "", type = "Team" });
        Assert.Equal("Team", workspace.Type);

        var project = await PostAsync<ProjectResponse>(client, "/api/projects", new
        {
            workspaceId = workspace.Id,
            name = "Sync project",
            description = "Description",
            color = "#123456",
            icon = "rocket",
            coverUrl = "https://example.com/cover.png",
            status = "OnHold",
            isArchived = false
        });
        Assert.Equal("#123456", project.Color);
        Assert.Equal("rocket", project.Icon);
        Assert.Equal("OnHold", project.Status);

        var board = await PostAsync<EntityResponse>(client, "/api/boards", new { projectId = project.Id, name = "Board" });
        var column = await PostAsync<EntityResponse>(client, "/api/columns", new { boardId = board.Id, name = "Todo", position = 0 });
        var task = await PostAsync<TaskItemResponse>(client, "/api/taskitems", new
        {
            projectId = project.Id,
            boardColumnId = column.Id,
            parentTaskItemId = (Guid?)null,
            title = "Sync task",
            description = "",
            position = 0,
            dueDateUtc = DateTime.UtcNow.AddDays(1),
            assigneeId = (Guid?)null,
            status = "InProgress",
            priority = "High"
        });

        Assert.Equal("InProgress", task.Status);
        Assert.Equal("High", task.Priority);

        await PostAsync<EntityResponse>(client, $"/api/taskitems/{task.Id}/comments", new { body = "Comment" });
        var checklist = await PostAsync<ChecklistResponse>(client, $"/api/taskitems/{task.Id}/checklist", new { text = "Check", position = 0 });
        await PostAsync<ChecklistResponse>(client, $"/api/checklist/{checklist.Id}/toggle", new { });
        var label = await PostAsync<LabelResponse>(client, $"/api/projects/{project.Id}/labels", new { boardId = board.Id, name = "Backend", colorHex = "#22c55e" });
        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync($"/api/taskitems/{task.Id}/labels/{label.Id}", null)).StatusCode);
        var note = await PostAsync<ProjectNoteResponse>(client, $"/api/projects/{project.Id}/notes", new { title = "Note", contentMarkdown = "Body" });
        Assert.Equal("Note", note.Title);

        var overview = await client.GetFromJsonAsync<ProjectOverviewResponse>($"/api/projects/{project.Id}/overview");
        Assert.Equal("#123456", overview!.Color);
        Assert.NotEmpty(overview.Notes);

        var details = await client.GetFromJsonAsync<TaskDetailsFullResponse>($"/api/taskitems/{task.Id}/details");
        Assert.Equal("InProgress", details!.Status);
        Assert.Equal("High", details.Priority);
        Assert.NotEmpty(details.Comments);
        Assert.NotEmpty(details.ChecklistItems);
        Assert.NotEmpty(details.Labels);
    }

    private static async Task<AuthResponse> RegisterAsync(HttpClient client, string email)
    {
        var register = await PostAsync<AuthResponse>(client, "/api/auth/register", new { email, password = "Password123!", displayName = email });
        var login = await PostAsync<AuthResponse>(client, "/api/auth/login", new { email, password = "Password123!" });
        Assert.Equal(register.UserId, login.UserId);
        return login;
    }

    private static async Task<T> PostAsync<T>(HttpClient client, string url, object body)
    {
        var response = await client.PostAsJsonAsync(url, body);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static async Task<T> PutAsync<T>(HttpClient client, string url, object body)
    {
        var response = await client.PutAsJsonAsync(url, body);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private sealed record AuthResponse(Guid UserId, string AccessToken);
    private sealed record WorkspaceResponse(Guid Id, string Type);
    private sealed record ProjectResponse(Guid Id, string? Color, string? Icon, string Status);
    private sealed record EntityResponse(Guid Id);
    private sealed record TaskItemResponse(Guid Id, Guid ProjectId, string? Status, string? Priority);
    private sealed record PagedResponse<T>(IReadOnlyList<T> Items);
    private sealed record ProjectOverviewResponse(Guid Id, string? Color, IReadOnlyList<ProjectNoteResponse> Notes);
    private sealed record KanbanResponse(IReadOnlyList<KanbanColumn> Columns);
    private sealed record KanbanColumn(IReadOnlyList<TaskItemResponse> Tasks);
    private sealed record TaskDetailsResponse(Guid Id);
    private sealed record TaskDetailsFullResponse(Guid Id, string Status, string Priority, IReadOnlyList<object> Comments, IReadOnlyList<object> ChecklistItems, IReadOnlyList<object> Labels);
    private sealed record ChecklistResponse(Guid Id);
    private sealed record LabelResponse(Guid Id);
    private sealed record ProjectNoteResponse(Guid Id, string Title);
}
