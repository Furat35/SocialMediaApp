using BuildingBlocks.Models;
using IdentityServer.Api.Business.Dtos.Followers;
using IdentityServer.Api.Core.Domain.Entities;
using IdentityServer.Api.Core.Domain.Enums;

namespace IdentityServer.Api.Business.Interfaces
{
    public interface IFollowerService
    {
        Task<bool> IsFollowing(int userId1, int userId2);
        Task<bool> ActiveUserHasAccessToGivenUser(int userId);
        Task<ResponseDto<bool>> Unfollow(int userId);
        Task<ResponseDto<bool>> SendFollowRequest(int userId);
        (bool isValid, string? message) CheckFollowStatus(Follower follower);
        Task<ResponseDto<bool>> RemoveBan(int userId);
        Task<ResponseDto<FollowerListDto>> GetFollowStatus(int userId);
        Task<PaginationResponseModel<FollowerListDto>> GetFollowRequests(PaginationRequestModel request);
        Task<PaginationResponseModel<FollowerListDto>> GetFollowersByUserIds(PaginationRequestModel request, List<int> UserIds);
        Task<PaginationResponseModel<FollowerListDto>> GetFollowersByUserId(int userId, FollowStatus status, PaginationRequestModel request);
        Task<ResponseDto<List<int>>> GetFollowerIds();
        Task<int> GetFollowersCount(int userId);
        Task<ResponseDto<bool>> AcceptFollowRequest(int userId);
        Task<ResponseDto<bool>> BanFollower(int userId);
        Task<ResponseDto<bool>> CancelFollowRequest(int userId);
        Task<ResponseDto<bool>> DeclineFollowRequest(int userId);
    }
}
