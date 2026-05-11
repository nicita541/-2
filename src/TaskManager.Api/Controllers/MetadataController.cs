using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class MetadataController : ApiControllerBase
{
    [HttpGet("task-options")]
    public IActionResult TaskOptions() => Ok(new
    {
        workspaceTypes = new[] { new { value = "Personal", label = "Личное" }, new { value = "Team", label = "Команда" } },
        projectStatuses = new[] { new { value = "Active", label = "Активный" }, new { value = "OnHold", label = "На паузе" }, new { value = "Completed", label = "Завершён" }, new { value = "Archived", label = "Архив" } },
        taskStatuses = new[] { new { value = "Todo", label = "Нужно сделать" }, new { value = "InProgress", label = "В процессе" }, new { value = "Review", label = "На проверке" }, new { value = "Done", label = "Готово" }, new { value = "Archived", label = "Архив" } },
        taskPriorities = new[] { new { value = "Low", label = "Низкий" }, new { value = "Medium", label = "Средний" }, new { value = "High", label = "Высокий" }, new { value = "Critical", label = "Критический" } }
    });
}
