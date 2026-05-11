# Task Manager Backend

Production-like ASP.NET Core backend for a task/project management application.

## Stack

- .NET 9
- ASP.NET Core Web API
- PostgreSQL
- Entity Framework Core with Fluent API
- JWT bearer authentication
- Swagger/OpenAPI
- Docker Compose
- Clean Architecture style: `Domain`, `Application`, `Infrastructure`, `Api`

## Features

- Auth: register/login with hashed passwords and JWT access tokens
- Users
- Workspaces and workspace roles
- Projects
- Boards and columns
- `TaskItem` entities, including infinite subtasks through self-referencing parent links
- Comments
- Attachments metadata
- Labels and task-label assignments
- Checklist items
- UTC audit timestamps
- Soft delete with EF Core query filters

## Run Locally

Start PostgreSQL:

```powershell
docker compose up -d postgres
```

Restore and build:

```powershell
dotnet restore
dotnet build
```

Run migrations after installing the EF tool:

```powershell
dotnet tool install --global dotnet-ef --version 9.0.4
dotnet ef migrations add InitialCreate --project src/TaskManager.Infrastructure --startup-project src/TaskManager.Api
dotnet ef database update --project src/TaskManager.Infrastructure --startup-project src/TaskManager.Api
```

If global tools are blocked by NuGet SSL/VPN issues, retry with a local manifest:

```powershell
dotnet new tool-manifest
dotnet tool install dotnet-ef --version 9.0.4
dotnet tool run dotnet-ef migrations add InitialCreate --project src/TaskManager.Infrastructure --startup-project src/TaskManager.Api --output-dir Persistence/Migrations
dotnet tool run dotnet-ef database update --project src/TaskManager.Infrastructure --startup-project src/TaskManager.Api
```

Run the API:

```powershell
dotnet run --project src/TaskManager.Api
```

Swagger is available at `https://localhost:<port>/swagger` in development.

## Docker

```powershell
docker compose up --build
```

API: `http://localhost:8080`

## Main Endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/workspaces`
- `POST /api/workspaces`
- `POST /api/workspaces/{workspaceId}/members`
- `GET /api/projects?workspaceId=...`
- `POST /api/projects`
- `GET /api/boards?projectId=...`
- `POST /api/boards`
- `GET /api/columns?boardId=...`
- `POST /api/columns`
- `GET /api/taskitems?boardColumnId=...&parentTaskItemId=...`
- `POST /api/taskitems`
- `PUT /api/taskitems/{id}`
- `DELETE /api/taskitems/{id}`
- `POST /api/comments`
- `POST /api/attachments`
- `POST /api/labels`
- `POST /api/labels/{labelId}/task-items/{taskItemId}`
- `POST /api/checklist`
- `PUT /api/checklist/{id}`

## Production Notes

- Replace every `Jwt__Secret` value with a strong secret from a secure store.
- Add authorization policies before exposing workspace data to untrusted clients.
- Store uploaded files outside the database; current attachments model stores metadata and URL.
- Add migrations to source control after the first database model is finalized.
- `TaskItem` contains `ProjectId`; clients must send it when creating/updating task items.
- Service methods enforce workspace/project permissions through `IPermissionService`.
