using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using IdentityServer.Api.Business.Dtos.Followers;
using IdentityServer.Api.Business.Interfaces;
using IdentityServer.Api.Core.Domain.Entities;
using IdentityServer.Api.Core.Domain.Enums;
using IdentityServer.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IdentityServer.Api.Business
{
    public class FollowerService(
        IFollowerRepository followerRepository,
        IHttpContextAccessor httpContext)
        : BaseService<IFollowerRepository, Follower>(followerRepository), IFollowerService
    {
        public async Task<bool> IsFollowing(int userId1, int userId2)
        {
            return await Repository.Get(_ => _.Status == FollowStatus.Following && _.IsValid &&
                (_.RequestingUserId == userId1 && _.RespondingUserId == userId2 ||
                 _.RequestingUserId == userId2 && _.RespondingUserId == userId1))
                .AnyAsync();
        }

        public async Task<bool> ActiveUserHasAccessToGivenUser(int userId)
        {
            var isFollowing = await IsFollowing(userId, httpContext.GetUserId());
            return httpContext.GetUserId() == userId || isFollowing;
        }

        public async Task<ResponseDto<bool>> Unfollow(int userId)
        {
            var follower = await Repository.GetFollowerByIdAsync(userId);
            if (follower is null)
                return ToResponse<bool>(HttpStatusCode.NotFound);

            follower.IsValid = false;
            return await SaveChangesAsync();
        }

        public async Task<ResponseDto<bool>> SendFollowRequest(int userId)
        {
            if (userId == httpContext.GetUserId())
                return ReturnFail<bool>("You cannot send a Follow request to yourself.", HttpStatusCode.BadRequest);
            var followExists = await Repository.FollowExistsAsync(userId);

            var result = CheckFollowStatus(followExists);
            if (!result.isValid) return ToResponse<bool>(HttpStatusCode.BadRequest);

            await Repository.AddAsync(
                new Follower
                {
                    RequestingUserId = httpContext.GetUserId(),
                    RespondingUserId = userId,
                    Status = FollowStatus.Pending,
                    IsValid = true
                });

            return await SaveChangesAsync();
        }

        public (bool isValid, string? message) CheckFollowStatus(Follower follower)
        {
            if (follower is not null)
            {
                var message = string.Empty;
                switch (follower.Status)
                {
                    case FollowStatus.Pending:
                        message = "Follow request already exists."; break;
                    case FollowStatus.Following:
                        message = "Already following."; break;
                    case FollowStatus.Declined:
                        if (follower.CreateDate.AddDays(7) > DateTime.UtcNow)
                            message = $"You cannot send a Follow request to this user within {(follower.CreateDate.AddDays(7) - DateTime.UtcNow).Days} days of declining the previous request.";
                        else return (true, null);
                        break;
                    case FollowStatus.Banned:
                        message = "You are banned. You can't send a request."; break;
                }
                return (false, message);
            }

            return (true, null);
        }

        public async Task<ResponseDto<bool>> RemoveBan(int userId)
        {
            var follower = await Repository.GetFollowerWithGivenStatusAsync(httpContext.GetUserId(), userId, FollowStatus.Banned);
            if (follower is null) return ToResponse<bool>(HttpStatusCode.BadRequest);

            follower.IsValid = false;

            return await SaveChangesAsync();
        }


        public async Task<ResponseDto<FollowerListDto>> GetFollowStatus(int userId)
        {
            var follower = await Repository.Get(_ => ((_.RequestingUserId == userId && _.RespondingUserId == httpContext.GetUserId()) ||
                     (_.RequestingUserId == httpContext.GetUserId() && _.RespondingUserId == userId)) &&
                      _.IsValid)
               .OrderByDescending(_ => _.CreateDate)
               .Select(_ => new FollowerListDto
               {
                   Id = _.Id,
                   RequestingUserId = _.RequestingUserId,
                   RespondingUserId = _.RespondingUserId,
                   Status = _.Status,
                   CreateDate = _.CreateDate
               }).FirstOrDefaultAsync();

            follower ??= new FollowerListDto
            {
                Id = null,
                RequestingUserId = null,
                RespondingUserId = null,
                Status = FollowStatus.NotFollowing,
                CreateDate = null
            };

            return ReturnSuccess(follower, HttpStatusCode.OK);
        }

        public async Task<PaginationResponseModel<FollowerListDto>> GetFollowRequests(PaginationRequestModel request)
        {
            var followerResponse = Repository.Get(_ => _.Status == FollowStatus.Pending && _.RespondingUserId == httpContext.GetUserId() && _.IsValid)
               .Select(_ => new FollowerListDto
               {
                   Id = _.Id,
                   RequestingUserId = _.RequestingUserId,
                   RespondingUserId = _.RespondingUserId,
                   Status = _.Status,
                   CreateDate = _.CreateDate
               });

            var totalfollowRequests = await followerResponse.CountAsync();
            var pageCount = (int)Math.Ceiling((double)totalfollowRequests / request.PageSize);

            var response = await followerResponse
                .OrderByDescending(_ => _.CreateDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var paginationModel = new PaginationResponseModel<FollowerListDto>(request.Page, request.PageSize, pageCount, totalfollowRequests, response);

            return paginationModel;
        }

        public async Task<PaginationResponseModel<FollowerListDto>> GetFollowersByUserIds(PaginationRequestModel request, List<int> UserIds)
        {
            var userIds = UserIds.Distinct().Except([httpContext.GetUserId()]).ToList();
            var followers = Repository.Get(_ => ((userIds.Contains(_.RequestingUserId) && _.RespondingUserId == httpContext.GetUserId()) ||
                            (userIds.Contains(_.RespondingUserId) && _.RequestingUserId == httpContext.GetUserId())) &&
                            _.Status == FollowStatus.Following && _.IsValid)
                        .Select(_ => new FollowerListDto
                        {
                            Id = _.Id,
                            RequestingUserId = _.RequestingUserId,
                            RespondingUserId = _.RespondingUserId,
                            Status = _.Status,
                            CreateDate = _.CreateDate
                        });

            var totalFollowers = await followers.CountAsync();
            var pageCount = (int)Math.Ceiling((double)totalFollowers / request.PageSize);

            var responseData = await followers
                .OrderByDescending(_ => _.CreateDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginationResponseModel<FollowerListDto>(request.Page, request.PageSize, pageCount, totalFollowers, responseData);
        }

        public async Task<PaginationResponseModel<FollowerListDto>> GetFollowersByUserId(int userId, FollowStatus status, PaginationRequestModel request)
        {
            var followers = Repository.Get(_ => (_.RequestingUserId == userId || _.RespondingUserId == userId) &&
                         _.Status == status && _.IsValid)
                    .OrderByDescending(_ => _.CreateDate)
                    .Select(_ => new FollowerListDto
                    {
                        Id = _.Id,
                        RequestingUserId = _.RequestingUserId,
                        RespondingUserId = _.RespondingUserId,
                        Status = _.Status,
                        CreateDate = _.CreateDate
                    });

            var totalFollowers = await followers.CountAsync();
            var pageCount = (int)Math.Ceiling((double)totalFollowers / request.PageSize);

            var responseData = await followers
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginationResponseModel<FollowerListDto>(request.Page, request.PageSize, pageCount, totalFollowers, responseData);
        }


        public async Task<ResponseDto<List<int>>> GetFollowerIds()
        {
            var followerIds = await Repository.Get(_ => (_.RequestingUserId == httpContext.GetUserId() || _.RespondingUserId == httpContext.GetUserId())
                                  && _.IsValid && _.Status == FollowStatus.Following)
                  .Select(_ => _.RequestingUserId == httpContext.GetUserId() ? _.RespondingUserId : _.RequestingUserId)
                  .ToListAsync();

            return ReturnSuccess(followerIds, HttpStatusCode.OK);
        }

        public async Task<int> GetFollowersCount(int userId)
        {
            var followerCount = await Repository.GetFollowersCountAsync(userId);
            return followerCount;
        }


        public async Task<ResponseDto<bool>> AcceptFollowRequest(int userId)
        {
            var follow = await Repository.GetFollowerWithGivenStatusAsync(userId, httpContext.GetUserId(), FollowStatus.Pending);
            if (follow is null || follow.Status == FollowStatus.Following)
                return ToResponse<bool>(HttpStatusCode.NotFound);

            follow.Status = FollowStatus.Following;

            return await SaveChangesAsync();
        }

        public async Task<ResponseDto<bool>> BanFollower(int userId)
        {
            var follower = await Repository.Get(_ => ((_.RequestingUserId == userId && _.RespondingUserId == httpContext.GetUserId()) ||
                                   (_.RequestingUserId == httpContext.GetUserId() && _.RespondingUserId == userId)) &&
                                    _.IsValid).FirstOrDefaultAsync();

            if (follower is not null) follower.IsValid = false;

            await Repository.AddAsync(
                new Follower
                {
                    RequestingUserId = httpContext.GetUserId(),
                    RespondingUserId = userId,
                    Status = FollowStatus.Banned,
                    IsValid = true
                });

            return await SaveChangesAsync();
        }

        public async Task<ResponseDto<bool>> CancelFollowRequest(int userId)
        {
            var follow = await Repository.GetFollowerWithGivenStatusAsync(httpContext.GetUserId(), userId, FollowStatus.Pending);
            if (follow is null) return ToResponse<bool>(HttpStatusCode.NotFound);

            follow.Status = FollowStatus.Cancelled;
            follow.IsValid = false;

            return await SaveChangesAsync();
        }

        public async Task<ResponseDto<bool>> DeclineFollowRequest(int userId)
        {
            var follow = await Repository.Get(_ => _.RequestingUserId == userId && _.RespondingUserId == httpContext.GetUserId() &&
                   _.Status == FollowStatus.Pending && _.IsValid)
               .FirstOrDefaultAsync();
            if (follow is null) return ToResponse<bool>(HttpStatusCode.NotFound);

            follow.Status = FollowStatus.Declined;
            follow.IsValid = false;

            return await SaveChangesAsync();
        }
    }
}
