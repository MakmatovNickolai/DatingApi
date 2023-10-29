using Common.Exceptions;
using DataAccess.Entities;
using DataAccess.Repositories;
using Infrastructure.Config;
using Infrastructure.Models.Response;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public interface IImageService
{
    Task<(byte[] FileData, string ContentType)?> GetImage(Guid id);
    Task<bool> DeleteImage(Guid id);
    Task<ImageResponse> InsertImage(string contentType, MemoryStream stream);
}
internal class ImageService : IImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ImageOptions _imageOptions;

    public ImageService(IOptions<ImageOptions> imageOptions, IUnitOfWork unitOfWork)
    {
        _imageOptions = imageOptions.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> DeleteImage(Guid id)
    {
        await _unitOfWork.ImageRepository.DeleteAsync(x => x.Id == id);
        return true;
    }

    public async Task<(byte[] FileData, string ContentType)?> GetImage(Guid id)
    {
        var imageData = await _unitOfWork.ImageRepository.GetAsync(id);

        if (imageData == null)
        {
            return null;
        }

        return (imageData.FileData, imageData.ContentType);
    }

    public async Task<ImageResponse> InsertImage(string contentType, MemoryStream stream)
    {
        if (stream.Length > _imageOptions.MaxSize)
        {
            throw new BadRequestException("File is too large");
        }

        var newImage = await _unitOfWork.ImageRepository.InsertAsync(new Image
        {
            Id = Guid.NewGuid(),
            ContentType = contentType,
            FileData = stream.ToArray(),
            IsTemporary = true
        });

        return new ImageResponse
        {
            Id = newImage.Id
        };
    }
}