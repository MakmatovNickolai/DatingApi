using DataAccess.Entities;

namespace FakeData;

public class FakeImage
{
    public List<string> Images { get; set; }
    private Random Random = new Random();
    public FakeImage()
    {
        Images = new List<string>();
        foreach (var imageExstension in Constants.ImageExtensions)
        {
            var pics = Directory.GetFiles(Constants.ImagesFolder, $"*{imageExstension}");
            Images.AddRange(pics);
        }
    }

    public Image GetNext(Guid? profileId = null)
    {
        byte[] imageBytes;
        if (Images.Count > 0)
        {
            imageBytes = File.ReadAllBytes(Images[Random.Next(Images.Count)]);
        }
        else
        {
            imageBytes = Array.Empty<byte>();
        }

        return new Image
        {
            Id = Guid.NewGuid(),
            ProfileId = profileId,
            ContentType = "image/jpeg",
            FileData = imageBytes
        };
    }
}