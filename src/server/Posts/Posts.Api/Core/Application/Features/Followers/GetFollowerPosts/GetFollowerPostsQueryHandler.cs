using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowerPosts
{
    public class GetFollowerPostsQueryHandler(IFollowerRepository followerRepository, IPostRepository postRepository,
        IHttpContextAccessor httpContext, IMapper mapper)
        : IRequestHandler<GetFollowerPostsQuery, PaginationResponseModel<PostListDto>>
    {
        public async Task<PaginationResponseModel<PostListDto>> Handle(GetFollowerPostsQuery request, CancellationToken cancellationToken)
        {
            var followers = followerRepository
                .Get(_ => _.Status == FollowStatus.Accepted && (_.RequestingUserId == httpContext.GetUserId() || _.RespondingUserId == httpContext.GetUserId()));

            var userPosts = postRepository
                .Get(_ => _.IsValid, includes: [i => i.Likes, i => i.Comments])
                .OrderByDescending(_ => _.CreateDate)
                .Join(
                    followers,
                    p => p.UserId,
                    f => f.RequestingUserId == httpContext.GetUserId() ? f.RespondingUserId : f.RequestingUserId,
                    (p, u) => p
                );

            var totalUserPosts = await userPosts.CountAsync();
            var pageCount = totalUserPosts / request.PageSize + (totalUserPosts % request.PageSize > 0 ? 1 : 0);

            var responsePosts = await userPosts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mappedData = mapper.Map<List<PostListDto>>(responsePosts);
            return new PaginationResponseModel<PostListDto>(request.Page, request.PageSize, pageCount, totalUserPosts, mappedData);
        }
    }
}
