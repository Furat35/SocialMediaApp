using Microsoft.AspNetCore.Http;

namespace IdentityServer.Api.Business.Interfaces
{
    public interface IFileService
    {
        void RemoveFile(string path);
        Task<string> SaveFileAsync(IFormFile file, string folder, string fileName = null);
        byte[] GetFile(string path);
    }
}
