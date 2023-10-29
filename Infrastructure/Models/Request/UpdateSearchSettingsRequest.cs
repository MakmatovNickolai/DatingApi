using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models.Request;

public class UpdateSearchSettingsRequest
{
    [Range(10, 510)]
    public int Distance { get; set; }
    [Range(18, 100)]
    public int AgeMin { get; set; }
    [Range(18, 100)]
    public int AgeMax { get; set; }
}