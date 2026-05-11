using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManager.Application.Common.Validation;

namespace TaskManager.Api.Filters;

public sealed class RequestValidationFilter(IEnumerable<IRequestValidator> validators) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new List<string>();

        foreach (var argument in context.ActionArguments.Values.Where(x => x is not null))
        {
            errors.AddRange(validators
                .Where(x => x.RequestType == argument!.GetType())
                .SelectMany(x => x.ValidateObject(argument!)));
        }

        if (errors.Count > 0)
        {
            context.Result = new BadRequestObjectResult(new
            {
                error = new
                {
                    code = "validation_error",
                    message = "Validation failed",
                    details = errors
                }
            });
            return;
        }

        await next();
    }
}
