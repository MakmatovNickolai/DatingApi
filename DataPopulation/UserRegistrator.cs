using FakeData;
using System.Net.Http.Json;

namespace FakeApiCallApp;

internal static class UserRegistrator
{
    private static Random Random = new Random();
    public static async Task RegisterEmailAsync(int count)
    {
        var latitude = Random.NextDouble() * 180 - 90;
        var longitude = Random.NextDouble() * 360 - 90;
        var initialCoordinate = new Infrastructure.Helpers.Geolocation.GeoCoordinate(latitude, longitude);
        var distanceMeters = 4000;
        for (int i = 0; i < count; i++)
        {
            using var httpClient = new HttpClient();
            var imageIds = await ImageUploader.UploadImagesAsync(2);
            var request = FakeRequest.RegisterEmail(imageIds);
            JsonContent content = JsonContent.Create(request);
            var res = await httpClient.PostAsync($"{Constants.ApiUrl}/auth/register/email", content);
            var resp = await res.Content.ReadAsStringAsync();
            Console.WriteLine(resp);

            var coordinate = LocationGenerator.GenerateRandomCoordinate(initialCoordinate, distanceMeters);
            var updateLocationRequest = FakeRequest.UpdateLocation(coordinate);
            var updateLocationtResponse = await httpClient.PostAsync($"{Constants.ApiUrl}/users/me/location", JsonContent.Create(updateLocationRequest));
            var respString = await updateLocationtResponse.Content.ReadAsStringAsync();
            Console.WriteLine(respString);
        }
    }
}
