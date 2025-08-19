using BuildingBlocks.Extensions;
using BuildingBlocks.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Services
{
    public class FileService(IHttpContextAccessor httpContext) : IFileService
    {
        public void RemoveFile(string path)
        {
            if (!File.Exists(path)) return;
            File.Delete(path);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder, string fileName = null)
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

        public byte[] GetFile(string path)
        {
            if (!File.Exists(path))
                throw new Exception("No file uploaded");

            return File.ReadAllBytes(path);
        }
    }
}
