using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Matserver.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            // The server knows where wwwroot is!
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // Always save as .jpg since we're converting to JPEG
            var fileName = Guid.NewGuid().ToString() + ".jpg";
            var filePath = Path.Combine(folderPath, fileName);

            // Load, resize, and save as compressed JPEG
            using (var inputStream = file.OpenReadStream())
            using (var image = await Image.LoadAsync(inputStream))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(100, 100),
                    Sampler = KnownResamplers.Lanczos3
                }));
                await image.SaveAsPngAsync(filePath);

                //image.Mutate(x => x.Resize(100, 100));
                //await image.SaveAsJpegAsync(filePath, new JpegEncoder { Quality = 80 });
            }

            // Return the relative path for the database
            return Ok(new { path = $"/images/{fileName}" });
        }
    }
}
