using System.Net.Http.Json;

namespace FakeData;

internal static class UserRegistrator
{
    public static async Task RegisterEmailAsync(int count)
    {
        for (int i = 0; i < count; i++)
        {
            using var httpClient = new HttpClient();
            var imageIds = await ImageUploader.UploadImagesAsync(2);
            var request = FakeRequest.RegisterEmail(imageIds);
            JsonContent content = JsonContent.Create(request);
            var res = await httpClient.PostAsync($"{Constants.ApiUrl}/auth/register/email", content);
            var resp = await res.Content.ReadAsStringAsync();
            Console.WriteLine(resp);
        }
    }
}
