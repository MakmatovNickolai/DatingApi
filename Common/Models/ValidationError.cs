using Newtonsoft.Json;

namespace Common.Models;

public class ValidationError
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Field { get; set; }

    public string Message { get; set; }

    public ValidationError(string field, string message)
    {
        Field = field != string.Empty ? field : null;
        Message = message;
    }
}
