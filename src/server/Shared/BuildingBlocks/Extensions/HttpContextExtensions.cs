using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BuildingBlocks.Extensions
{
    public static class HttpContextExtensions
    {
        public static int GetUserId(this IHttpContextAccessor httpContext)
        {
            return int.Parse(httpContext.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public static string GetFullname(this IHttpContextAccessor httpContext)
        {
            return httpContext.HttpContext.User.Claims.First(c => c.Type == "fullname").Value;
        }

        public static IFormFile? GetFirstFormFile(this IHttpContextAccessor httpContext)
        {
            return httpContext.HttpContext.Request.Form.Files.FirstOrDefault();
        }
        public static IFormFileCollection GetFormFiles(this IHttpContextAccessor httpContext)
        {
            return httpContext.HttpContext.Request.Form.Files;
        }
    }
}
