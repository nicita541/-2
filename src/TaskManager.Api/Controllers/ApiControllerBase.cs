using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<T> ToActionResult<T>(Result<T> result)
    {
        if (result.Succeeded)
        {
            return Ok(result.Value);
        }

        return result.Error?.Code switch
        {
            "not_found" => NotFound(ToEnvelope(result.Error)),
            "unauthorized" => Unauthorized(ToEnvelope(result.Error)),
            "forbidden" => Forbid(),
            "conflict" => Conflict(ToEnvelope(result.Error)),
            _ => BadRequest(ToEnvelope(result.Error ?? new Error("bad_request", "Bad request")))
        };
    }

    protected IActionResult ToNoContentResult(Result<bool> result)
    {
        if (result.Succeeded) return NoContent();

        return result.Error?.Code switch
        {
            "not_found" => NotFound(ToEnvelope(result.Error)),
            "unauthorized" => Unauthorized(ToEnvelope(result.Error)),
            "forbidden" => Forbid(),
            "conflict" => Conflict(ToEnvelope(result.Error)),
            _ => BadRequest(ToEnvelope(result.Error ?? new Error("bad_request", "Bad request")))
        };
    }

    private static object ToEnvelope(Error error) => new { error = new { error.Code, error.Message, details = error.Details ?? [] } };
}
