using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Common.Models;

public class ValidationErrorResponse : BaseResponse
{

    public List<ValidationError> Errors { get; set; }
    public ValidationErrorResponse()
    {

    }

    public ValidationErrorResponse(ModelStateDictionary modelState)
    {
        Success = false;
        Message = "Validation Failed";
        Errors = modelState
            .Keys
            .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
            .ToList();
    }
}
