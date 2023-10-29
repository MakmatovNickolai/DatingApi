namespace DataAccess.Models;

public class SearchOptions
{
    public int Distance { get; set; }
    public int AgeMin { get; set; }
    public int AgeMax { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Guid ProfileId { get; set; }
    public int Limit { get; set; }
}