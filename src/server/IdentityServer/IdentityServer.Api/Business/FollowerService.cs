using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using IdentityServer.Api.Business.Dtos.Followers;
using IdentityServer.Api.Business.Interfaces;
using IdentityServer.Api.Core.Domain.Entities;
using IdentityServer.Api.Core.Domain.Enums;
using IdentityServer.Api.Data.Context;
using IdentityServer.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IdentityServer.Api.Business
{
    public class FollowerService(IdentityDbContext dbContext, IHttpContextAccessor httpContext)
        : FollowerRepository(dbContext, httpContext), IFollowerService
    {
        public async Task<ResponseDto<bool>> Unfollow(int userId)
        {
            var follower = await Get(f => (f.RequestingUserId == userId && f.RespondingUserId == httpContext.GetUserId() && f.Status == FollowStatus.Following) ||
                     (f.RespondingUserId == userId && f.RequestingUserId == httpContext.GetUserId()) && f.Status == FollowStatus.Following)
             .FirstOrDefaultAsync();
            if (follower is null)
                return ResponseDto<bool>.Fail("Follow does not exist.", HttpStatusCode.BadRequest);

            follower.IsValid = false;
            return ResponseDto<bool>
              .GenerateResponse(await SaveChangesAsync() > 0)
              .Success(true, HttpStatusCode.OK)
              .Fail("An error occured while deleting the Follow", HttpStatusCode.InternalServerError);
        }

        public async Task<ResponseDto<bool>> SendFollowRequest(int userId)
        {
            if (userId == httpContext.GetUserId())
                return ResponseDto<bool>.Fail("You cannot send a Follow request to yourself.", HttpStatusCode.BadRequest);
            var followExists = await Get(f => ((f.RequestingUserId == httpContext.GetUserId() && f.RespondingUserId == userId) ||
                          (f.RequestingUserId == userId && f.RespondingUserId == httpContext.GetUserId())) &&
                          f.IsValid)
                .OrderByDescending(_ => _.CreateDate)
                .FirstOrDefaultAsync();

            var result = await CheckFollowStatus(followExists);
            if (!result.isValid) return ResponseDto<bool>.Fail(result.message, HttpStatusCode.BadRequest);

            await AddAsync(
                new Follower
                {
                    RequestingUserId = httpContext.GetUserId(),
                    RespondingUserId = userId,
                    Status = FollowStatus.Pending,
                    IsValid = true
                });

            return ResponseDto<bool>
                .GenerateResponse(await SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.Created)
                .Fail("An error occured while sending the Follow request", HttpStatusCode.InternalServerError);
        }

        public async Task<(bool isValid, string? message)> CheckFollowStatus(Follower follower)
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
            var follower = await Get(_ => (_.RequestingUserId == httpContext.GetUserId() && _.RespondingUserId == userId)
                    && _.IsValid && _.Status == FollowStatus.Banned)
              .FirstOrDefaultAsync();

            if (follower is null)
                return ResponseDto<bool>.Fail("Ban does not exist.", HttpStatusCode.BadRequest);

            follower.IsValid = false;
            return ResponseDto<bool>
                .GenerateResponse(await SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail("An error occured while accepting the Follow request", HttpStatusCode.InternalServerError);
        }


        public async Task<ResponseDto<FollowerListDto>> GetFollowStatus(int userId)
        {
            var follower = await Get(_ => ((_.RequestingUserId == userId && _.RespondingUserId == httpContext.GetUserId()) ||
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

            return ResponseDto<FollowerListDto>.Success(follower, HttpStatusCode.OK);
        }

        public async Task<PaginationResponseModel<FollowerListDto>> GetFollowRequests(PaginationRequestModel request)
        {
            var followerResponse = Get(_ => _.Status == FollowStatus.Pending && _.RespondingUserId == httpContext.GetUserId() && _.IsValid)
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

            //var mappedData = mapper.Map<List<FollowerListDto>>(response);
            var paginationModel = new PaginationResponseModel<FollowerListDto>(request.Page, request.PageSize, pageCount, totalfollowRequests, response);

            return paginationModel;
        }

        public async Task<PaginationResponseModel<FollowerListDto>> GetFollowersByUserIds(PaginationRequestModel request, List<int> UserIds)
        {
            var userIds = UserIds.Distinct().Except([httpContext.GetUserId()]).ToList();
            var followers = Get(_ => ((userIds.Contains(_.RequestingUserId) && _.RespondingUserId == httpContext.GetUserId()) ||
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
            var followers = Get(_ => (_.RequestingUserId == userId || _.RespondingUserId == userId) &&
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
            var followerIds = await Get(_ => (_.RequestingUserId == httpContext.GetUserId() || _.RespondingUserId == httpContext.GetUserId())
                                  && _.IsValid && _.Status == FollowStatus.Following)
                  .Select(_ => _.RequestingUserId == httpContext.GetUserId() ? _.RespondingUserId : _.RequestingUserId)
                  .ToListAsync();

            return ResponseDto<List<int>>.Success(followerIds, HttpStatusCode.OK);
        }

        public async Task<int> GetFollowersCount()
        {
            var followerCount = await Get(_ => (_.RequestingUserId == httpContext.GetUserId() ||
                       _.RespondingUserId == httpContext.GetUserId()) &&
                       _.IsValid && _.Status == FollowStatus.Following)
                   .CountAsync();

            return followerCount;
        }


        public async Task<ResponseDto<bool>> AcceptFollowRequest(int userId)
        {
            var follow = await
                Get(f => f.RequestingUserId == userId && f.RespondingUserId == httpContext.GetUserId() &&
                    f.Status == FollowStatus.Pending && f.IsValid)
                .FirstOrDefaultAsync();
            if (follow is null)
                return ResponseDto<bool>.Fail("Follow request does not exist.", HttpStatusCode.BadRequest);

            if (follow.Status == FollowStatus.Following)
                return ResponseDto<bool>.Fail("Is already following.", HttpStatusCode.BadRequest);

            follow.Status = FollowStatus.Following;

            return ResponseDto<bool>
             .GenerateResponse(await SaveChangesAsync() > 0)
             .Success(true, HttpStatusCode.OK)
             .Fail("An error occured while accepting the Follow request", HttpStatusCode.InternalServerError);
        }

        public async Task<ResponseDto<bool>> BanFollower(int userId)
        {
            var follower = await Get(_ => ((_.RequestingUserId == userId && _.RespondingUserId == httpContext.GetUserId()) ||
                                   (_.RequestingUserId == httpContext.GetUserId() && _.RespondingUserId == userId)) &&
                                    _.IsValid).FirstOrDefaultAsync();

            if (follower is not null) follower.IsValid = false;

            await AddAsync(
                new Follower
                {
                    RequestingUserId = httpContext.GetUserId(),
                    RespondingUserId = userId,
                    Status = FollowStatus.Banned,
                    IsValid = true
                });

            return ResponseDto<bool>
                .GenerateResponse(await SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail("An error occured while accepting the Follow request", HttpStatusCode.InternalServerError);
        }

        public async Task<ResponseDto<bool>> CancelFollowRequest(int userId)
        {
            var follow = await Get(_ => _.RequestingUserId == httpContext.GetUserId() && _.RespondingUserId == userId &&
                      _.Status == FollowStatus.Pending && _.IsValid).FirstOrDefaultAsync();
            if (follow is null)
                return ResponseDto<bool>.Fail("Follow request does not exist.", HttpStatusCode.BadRequest);

            follow.Status = FollowStatus.Cancelled;
            follow.IsValid = false;

            return ResponseDto<bool>
                .GenerateResponse(await SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail("An error occured while canceling the Follow request", HttpStatusCode.InternalServerError);
        }

        public async Task<ResponseDto<bool>> DeclineFollowRequest(int userId)
        {
            var follow = await Get(_ => _.RequestingUserId == userId && _.RespondingUserId == httpContext.GetUserId() &&
                   _.Status == FollowStatus.Pending && _.IsValid)
               .FirstOrDefaultAsync();
            if (follow is null)
                return ResponseDto<bool>.Fail("Follow request does not exist.", HttpStatusCode.BadRequest);

            follow.Status = FollowStatus.Declined;
            follow.IsValid = false;

            return ResponseDto<bool>
             .GenerateResponse(await SaveChangesAsync() > 0)
             .Success(true, HttpStatusCode.OK)
             .Fail("An error occured while sending the Follow request", HttpStatusCode.InternalServerError);
        }
    }
}
