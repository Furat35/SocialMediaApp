using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Friends.GetFollowerPosts
{
    public class GetFollowerPostsQueryHandler(IFriendRepository friendRepository, IPostRepository postRepository, IHttpContextAccessor httpContext, IMapper mapper)
        : IRequestHandler<GetFollowerPostsQuery, ResponseDto<PaginationResponseModel<PostListDto>>>
    {
        public async Task<ResponseDto<PaginationResponseModel<PostListDto>>> Handle(GetFollowerPostsQuery request, CancellationToken cancellationToken)
        {
            var friends = friendRepository
                .Get(_ => _.IsValid && (_.RequestingUserId == httpContext.GetUserId() || _.RespondingUserId == httpContext.GetUserId()));

            var userPosts = postRepository
                .Get(_ => _.IsValid)
                .OrderByDescending(_ => _.CreateDate)
                .Join(
                    friends,
                    p => p.UserId,
                    f => f.RespondingUserId,
                    (p, u) => p
                );

            var totalUserPosts = await userPosts.CountAsync();
            var pageCount = totalUserPosts / request.PageSize + (totalUserPosts % request.PageSize > 0 ? 1 : 0);

            var responsePosts = await userPosts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            var mappedData = mapper.Map<List<PostListDto>>(responsePosts);
            var paginationModel = new PaginationResponseModel<PostListDto>(request.Page, request.PageSize, pageCount, mappedData);

            return ResponseDto<PaginationResponseModel<PostListDto>>.Success(paginationModel, HttpStatusCode.OK);
        }
    }
}
