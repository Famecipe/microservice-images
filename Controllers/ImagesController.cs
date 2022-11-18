using Microsoft.AspNetCore.Mvc;

namespace Famecipe.Microservice.Images.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
public class ImagesController : ControllerBase
{
    private readonly string _imageStoragePath;

    public ImagesController()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IMAGES_DATA_SOURCE")))
        {
            this._imageStoragePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Images"));
        }
        else
        {
            this._imageStoragePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, Environment.GetEnvironmentVariable("IMAGE_DATA_SOURCE")!));
        }

        if (!Directory.Exists(this._imageStoragePath))
        {
            Directory.CreateDirectory(this._imageStoragePath);
        }
    }

    [HttpGet("{identifier}")]
    public async Task<IActionResult> Get(string identifier)
    {
        Response.Headers.ContentEncoding = "gzip";
        await Task.CompletedTask;
        return File(System.IO.File.ReadAllBytes(Path.Combine(this._imageStoragePath, (identifier + ".gz"))), "image/jpeg");
    }

    [HttpPost]
    public async Task<IActionResult> Post(IFormFile image)
    {
        string identifier = Guid.NewGuid().ToString();
        try
        {
            if (image.Length > 0)
            {
                using (FileStream gzipFileStream = new FileInfo(Path.Combine(this._imageStoragePath, identifier + ".gz")).Create())
                {
                    using (System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(gzipFileStream, System.IO.Compression.CompressionMode.Compress))
                    {
                        image.CopyTo(gzipStream);
                    }
                }

                await Task.CompletedTask;
                return Ok(identifier);
            }
            else
            {
                return BadRequest();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("{identifier}")]
    public async Task<IActionResult> Put(string identifier, IFormFile image)
    {
        try
        {
            await Delete(identifier);

            using (FileStream gzipFileStream = new FileInfo(Path.Combine(this._imageStoragePath, identifier + ".gz")).Create())
            {
                using (System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(gzipFileStream, System.IO.Compression.CompressionMode.Compress))
                {
                    image.CopyTo(gzipStream);
                }
            }

            await Task.CompletedTask;

            return NoContent();
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpDelete("{identifier}")]
    public async Task<IActionResult> Delete(string identifier)
    {
        try
        {
            string image = Path.Combine(this._imageStoragePath, identifier + ".gz");
            System.IO.File.Delete(image);
            await Task.CompletedTask;
            return NoContent();
        }
        catch(Exception)
        {
            throw;
        }
    }
}
