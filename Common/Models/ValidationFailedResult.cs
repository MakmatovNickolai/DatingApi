using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Common.Models;

public class ValidationFailedResult : ObjectResult
{
    public ValidationFailedResult(ModelStateDictionary modelState) : base(new ValidationErrorResponse(modelState))
    {
        StatusCode = StatusCodes.Status422UnprocessableEntity; // change the http status code to 422.
    }
}
