using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace FakeApiCallApp;

internal static class ImageUploader
{
    public static async Task<Guid[]> UploadImagesAsync(int count)
    {
        var imageIds = new Guid[count];
        for (int i = 0; i < count; i++)
        {
            var rand = new Random();
            var images = new List<string>();
            foreach (var imageExstension in Constants.ImageExtensions)
            {
                var pics = Directory.GetFiles(Constants.ImagesFolder, $"*{imageExstension}");
                images.AddRange(pics);
            }

            var imagePath = images[rand.Next(images.Count)];
            var imageBytes = File.ReadAllBytes(imagePath);
            var uploadImageResult = await UploadAsync($"{Constants.ApiUrl}/images", imageBytes, imagePath);
            Console.WriteLine($"Upload image result {uploadImageResult}");
            var imageId = Guid.Parse(Regex.Match(uploadImageResult, "\"id\":\"([^\"]+)\"").Groups[1].Value);
            imageIds[i] = imageId;
        }
        return imageIds;
    }

    private static async Task<string> UploadAsync(string url, byte[] image, string filePath)
    {
        using (var client = new HttpClient())
        {
            using (var form = new MultipartFormDataContent())
            {
                using (var streamContent = new StreamContent(new MemoryStream(image)))
                {
                    using (var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync()))
                    {
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                        // "file" parameter name should be the same as the server side input parameter name
                        form.Add(fileContent, "file", Path.GetFileName(filePath));
                        using (var response = await client.PostAsync(url, form))
                        {
                            var input = await response.Content.ReadAsStringAsync();

                            return input;
                        }
                    }
                }
            }
        }
    }
}
