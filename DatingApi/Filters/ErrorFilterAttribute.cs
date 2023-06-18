using Common.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApi.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new ValidationFailedResult(context.ModelState);
        }
    }
}
