namespace FakeData;

internal static class Constants
{
    public const string BackendUrl = "http://localhost:5143";
    public const string ApiVersion = "v1";
    public const string ImagesFolder = "D:\\Downloads\\Telegram Desktop\\pics";
    public readonly static string[] ImageExtensions = new string[] { ".jpg", ".jpeg", ".png" };
    public static string ApiUrl => $"{BackendUrl}/api/{ApiVersion}";
}
