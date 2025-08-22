using AutoMapper;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using Posts.Api.ExternalServices;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.GetPostsByUserId
{
    public class GetPostsByUserIdQueryHandler(
        IPostRepository postRepository,
        IFollowerService followerService,
        IMapper mapper)
        : IRequestHandler<GetPostsByUserIdQuery, PaginationResponseModel<PostListDto>>
    {
        public async Task<PaginationResponseModel<PostListDto>> Handle(GetPostsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userPosts = postRepository
                .Get(_ => _.UserId == request.UserId && _.IsValid, includes: [i => i.Likes, i => i.Comments]);

            var totalUserPosts = await userPosts.CountAsync();
            var pageCount = (int)Math.Ceiling((double)totalUserPosts / request.PageSize);

            var response = await userPosts
                .OrderByDescending(_ => _.CreateDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mappedData = mapper.Map<List<PostListDto>>(response);
            return new PaginationResponseModel<PostListDto>(request.Page, request.PageSize, pageCount, totalUserPosts,
                await followerService.IsFollowing(request.UserId) ? mappedData : null);
        }
    }
}
