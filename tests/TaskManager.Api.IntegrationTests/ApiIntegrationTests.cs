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
    private sealed record EntityResponse(Guid Id);
    private sealed record TaskItemResponse(Guid Id, Guid ProjectId);
    private sealed record PagedResponse<T>(IReadOnlyList<T> Items);
    private sealed record ProjectOverviewResponse(Guid Id);
    private sealed record KanbanResponse(IReadOnlyList<KanbanColumn> Columns);
    private sealed record KanbanColumn(IReadOnlyList<TaskItemResponse> Tasks);
    private sealed record TaskDetailsResponse(Guid Id);
}
