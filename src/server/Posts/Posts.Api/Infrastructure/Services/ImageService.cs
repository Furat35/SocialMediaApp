using BuildingBlocks.Extensions;
using Posts.Api.Core.Application.Services;

namespace Posts.Api.Infrastructure.Services
{
    public class ImageService(IHttpContextAccessor httpContext) : IImageService
    {
        public void RemoveImage(string path)
        {
            if (!File.Exists(path)) return;
            File.Delete(path);
        }

        public async Task<string> SaveImageAsync(IFormFile file, string folder = "images/users", string fileName = null)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), folder, httpContext.GetUserId().ToString());
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var filePath = Path.Combine(uploadsFolder, fileName ?? file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public byte[] GetImage(string path)
        {
            if (!File.Exists(path))
                throw new Exception("No file uploaded");

            return File.ReadAllBytes(path);
        }
    }
}
