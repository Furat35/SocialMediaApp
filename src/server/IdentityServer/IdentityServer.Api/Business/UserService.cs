using BuildingBlocks.Extensions;
using BuildingBlocks.Interfaces.Services;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using IdentityServer.Api.Business.Dtos;
using IdentityServer.Api.Business.Dtos.AppUsers;
using IdentityServer.Api.Business.Interfaces;
using IdentityServer.Api.Data.Repository;
using IdentityServer.Api.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IdentityServer.Api.Business
{
    public class UserService(
        IUserRepository userRepository,
        IHttpContextAccessor httpContext,
        IServiceProvider serviceProvider,
        IFileService fileService)
        : BaseService<IUserRepository, AppUser>(userRepository), IUserService
    {
        public readonly Lazy<IAuthService> _authService = new(() => serviceProvider.GetRequiredService<IAuthService>());
        public async Task<PaginationResponseModel<AppUser>> GetUsers(UserRequestDto request)
        {
            var users = Repository.Get(_ => _.IsValid);

            if (request.SearchKey is not null)
                users = users.Where(_ => _.Fullname.Contains(request.SearchKey) || _.Username.Contains(request.SearchKey));

            var totalUsers = await users.CountAsync();
            var pageCount = (int)Math.Ceiling((double)totalUsers / request.PageSize);

            var response = await users
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginationResponseModel<AppUser>(request.Page, request.PageSize, pageCount, totalUsers, response);
        }
        public async Task<ResponseDto<List<int>>> SearchedUserIds(UserRequestDto request)
        {
            if (string.IsNullOrEmpty(request.SearchKey))
                return ToResponse<List<int>>(HttpStatusCode.BadRequest);

            var userIds = await Repository.Get(_ => _.IsValid && (_.Fullname.Contains(request.SearchKey) || _.Username.Contains(request.SearchKey)))
                .Select(_ => _.Id)
                .ToListAsync();

            return ReturnSuccess(userIds, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<AppUser>> GetUserById(int userId)
        {
            var user = await Repository.GetByIdAsync(userId);
            return ReturnSuccess(user, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<List<AppUser>>> GetUsersById(List<int> userIds)
        {
            var users = await Repository.Get(_ => userIds.Contains(_.Id)).ToListAsync();
            return ReturnSuccess(users, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<bool>> UpdateUser(AppUserUpdateDto updateDto)
        {
            var user = await Repository.GetById(httpContext.GetUserId());
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
                if (file is not null)
                {
                    path = await fileService.SaveFileAsync(file, $"images/users/profile/{httpContext.GetUserId().ToString()}");
                    user.ProfileImage = path;
                }
            }
            catch (Exception ex)
            {
                fileService.RemoveFile(path);
                return ReturnFail<bool>(ex.Message, HttpStatusCode.InternalServerError);
            }

            return await SaveChangesAsync();
        }

        public async Task<(byte[] image, string fileType)> GetUserImage(int userId)
        {
            var user = await Repository.GetByIdAsync(userId);
            if (!File.Exists(user.ProfileImage))
                throw new Exception(ErrorMessages.FileNotFound);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(user.ProfileImage, out var contentType))
            {
                contentType = "application/octet-stream"; // fallback for unknown types
            }


            return (File.ReadAllBytes(user.ProfileImage), contentType);
        }
    }
}
