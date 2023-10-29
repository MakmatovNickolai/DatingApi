namespace Infrastructure.Helpers.Geolocation;

public class DistanceCalculator
{
    private const double EarthRadiusKm = 6371;

    public static double CalculateDistance(GeoCoordinate coord1, GeoCoordinate coord2)
    {
        double dLat = DegreeToRadian(coord2.Latitude - coord1.Latitude);
        double dLon = DegreeToRadian(coord2.Longitude - coord1.Longitude);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreeToRadian(coord1.Latitude)) * Math.Cos(DegreeToRadian(coord2.Latitude)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double DegreeToRadian(double angle)
    {
        return Math.PI * angle / 180.0;
    }
}
