using Azure.Core;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using IdentityServer.Api.Business.Dtos.AppUsers;
using IdentityServer.Api.Business.Interfaces;
using IdentityServer.Api.Data.Context;
using IdentityServer.Api.Data.Repository;
using IdentityServer.Api.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IdentityServer.Api.Business
{
    public class UserService(IdentityDbContext context, IHttpContextAccessor httpContext, 
        IServiceProvider serviceProvider, IFileService fileService) 
        : UserRepository(context), IUserService
    {
        public readonly Lazy<IAuthService> _authService = new Lazy<IAuthService>(() => serviceProvider.GetRequiredService<IAuthService>());
        public async Task<ResponseDto<AppUser>> GetUserById(int userId)
        {
            var user = await GetByIdAsync(userId);
            return ResponseDto<AppUser>.Success(user, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<List<AppUser>>> GetUsersById(List<int> userIds)
        {
            var users = await Get(_ => userIds.Contains( _.Id)).ToListAsync();
            return ResponseDto<List<AppUser>>.Success(users, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<bool>> UpdateUser(AppUserUpdateDto updateDto)
        {
            var user = await GetById(httpContext.GetUserId());
            //validation check must be added

            user.Username = updateDto.Username;
            user.Fullname = updateDto.Fullname;
            user.Bio = updateDto.Bio;
            if (updateDto.Password != null)
            {
                var passwordSalt = Guid.NewGuid();
                byte[] salt = passwordSalt.ToByteArray();
                user.PasswordSalt = passwordSalt;
                user.HashedPassword = _authService.Value.HashPassword(updateDto.Password, salt);
            }
            user.IsValid = true;

            string path = string.Empty;
            try
            {
                var file = updateDto.ProfileImage;
                if(file is not null)
                {
                    path = await fileService.SaveFileAsync(file, $"images/users/profile/{httpContext.GetUserId().ToString()}");
                    user.ProfileImage = path;
                }
            }
            catch(Exception ex)
            {
                fileService.RemoveFile(path);
                return ResponseDto<bool>.Fail(ex.Message, HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.GenerateResponse(await SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail("An error occured during save operation", HttpStatusCode.InternalServerError);
        }

        public async Task<(byte[] image, string fileType)> GetUserImage(int userId)
        {
            var user = await GetByIdAsync(userId);
            if (!File.Exists(user.ProfileImage))
                throw new Exception("File doesn't exist");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(user.ProfileImage, out var contentType))
            {
                contentType = "application/octet-stream"; // fallback for unknown types
            }


            return (File.ReadAllBytes(user.ProfileImage), contentType);
        }
    }
}
