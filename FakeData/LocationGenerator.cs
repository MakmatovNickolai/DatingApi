using Infrastructure.Helpers.Geolocation;

namespace FakeData;

public static class LocationGenerator
{
    private const double EarthRadiusMeters = 6371000; // Earth radius in meters

    private static  Random Random = new Random();

    public static GeoCoordinate GenerateRandomCoordinate(GeoCoordinate initialLocation, double maxDistanceMeters)
    {
        // Convert maxDistance from meters to radians
        double maxDistanceRadians = maxDistanceMeters / EarthRadiusMeters;

        // Generate a random angle (in radians) for the new location
        double angle = Random.NextDouble() * 2 * Math.PI;

        // Generate a random distance (in radians) within the specified radius
        double distance = Math.Acos(Random.NextDouble() * (Math.Cos(maxDistanceRadians) - 1) + 1);

        // Calculate the new latitude and longitude
        double newLatitude = initialLocation.Latitude + (distance / Math.PI) * (180.0 / Math.PI) * Math.Cos(angle);
        double newLongitude = initialLocation.Longitude + (distance / Math.PI) * (180.0 / Math.PI) * Math.Sin(angle);

        return new GeoCoordinate(newLatitude, newLongitude);
    }
}
