using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Interfaces.Services
{
    public interface IFileService
    {
        void RemoveFile(string path);
        Task<string> SaveFileAsync(IFormFile file, string folder, string fileName = null);
        public byte[] GetFile(string path);
    }
}
