namespace Posts.Api.Core.Application.Services
{
    public interface IImageService
    {
        void RemoveImage(string path);
        Task<string> SaveImageAsync(IFormFile file, string folder = "images/users", string fileName = null);
        public byte[] GetImage(string path);
    }
}
