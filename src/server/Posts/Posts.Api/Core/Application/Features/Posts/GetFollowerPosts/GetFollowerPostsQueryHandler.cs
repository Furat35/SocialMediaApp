using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.ExternalServices;

namespace Posts.Api.Core.Application.Features.Posts.GetFollowerPosts
{
    public class GetFollowerPostsQueryHandler(IPostRepository postRepository,
        IHttpContextAccessor httpContext, IFollowerService followerService,
        IMapper mapper)
        : IRequestHandler<GetFollowerPostsQuery, PaginationResponseModel<PostListDto>>
    {
        public async Task<PaginationResponseModel<PostListDto>> Handle(GetFollowerPostsQuery request, CancellationToken cancellationToken)
        {
            var followerResponse = await followerService.GetFollowerIdsAsync();

            var userPosts = postRepository
                .Get(_ => _.IsValid, includes: [i => i.Likes, i => i.Comments])
                .OrderByDescending(_ => _.CreateDate)
                .Join(
                    followerResponse,
                    p => p.UserId,
                    f => f,
                    (p, u) => p
                );

            var totalUserPosts = await userPosts.CountAsync();
            var pageCount = (int)Math.Ceiling((double)totalUserPosts / request.PageSize);

            var responsePosts = await userPosts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mappedData = mapper.Map<List<PostListDto>>(responsePosts);
            return new PaginationResponseModel<PostListDto>(request.Page, request.PageSize, pageCount, totalUserPosts, mappedData);
        }
    }
}
