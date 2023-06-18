using Auth.Dto;
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
            var registerEmailRequest = new RegisterEmailRequest
            {
                Email = Faker.Internet.Email(),
                Password = "123456",
                AppVersion = "v1",
                BirthDate = Faker.Identification.DateOfBirth(),
                Description = Faker.Lorem.Paragraph(),
                DeviceGmtOffset = "0",
                DeviceInfo = "Windows 10",
                DeviceLocale = "en",
                DeviceUniqueId = Faker.RandomNumber.Next().ToString(),
                Gender = "Female",
                IsDeviceEmulator = false,
                IsDeviceRooted = false,
                Name = Faker.Name.First(),
                Platform = "Windows",
                ImageIds = imageIds
            };
            JsonContent content = JsonContent.Create(registerEmailRequest);
            var res = await httpClient.PostAsync($"{Constants.ApiUrl}/auth/register/email", content);
            var resp = await res.Content.ReadAsStringAsync();
            Console.WriteLine(resp);
        }
    }
}
