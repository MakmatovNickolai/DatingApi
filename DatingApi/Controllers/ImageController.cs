using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace DatingApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/images")]
// TODO authenticate upload requests and store images temporarily, only after registrion move them to permanent storage. consider rate limiting
// [Authorize]
[ApiVersion("1.0")]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;
    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    /// <summary>
    /// Uploads a new image
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImageResponse))]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        var ms = new MemoryStream();
        try
        {
            await file.CopyToAsync(ms);
            var imageData = await _imageService.InsertImage(file.ContentType, ms);
            return Ok(imageData);
        }
        finally
        {
            ms.Dispose();
        }
    }

    /// <summary>
    /// Gets an image
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetImage([FromRoute] Guid id)
    {
        var data = await _imageService.GetImage(id);
        if (!data.HasValue)
        {
            return NotFound();
        }

        return File(data.Value.FileData, data.Value.ContentType);
    }

    /// <summary>
    /// Deletes an image
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteImage([FromRoute] Guid id)
    {
        var result = await _imageService.DeleteImage(id);
        return Ok(result);
    }
}
