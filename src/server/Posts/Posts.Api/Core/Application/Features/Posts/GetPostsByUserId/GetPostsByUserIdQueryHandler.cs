using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Repositories;

namespace Posts.Api.Core.Application.Features.Posts.GetPostsByUserId
{
    public class GetPostsByUserIdQueryHandler(IPostRepository postRepository, IMapper mapper)
        : IRequestHandler<GetPostsByUserIdQuery, PaginationResponseModel<PostListDto>>
    {
        public async Task<PaginationResponseModel<PostListDto>> Handle(GetPostsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userPosts = postRepository
                .Get(_ => _.UserId == request.UserId && _.IsValid, includes: [i => i.Likes, i => i.Comments]);

            var totalUserPosts = await userPosts.CountAsync();
            var pageCount = totalUserPosts / request.PageSize + (totalUserPosts % request.PageSize > 0 ? 1 : 0);

            var response = await userPosts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mappedData = mapper.Map<List<PostListDto>>(response);
            return new PaginationResponseModel<PostListDto>(request.Page, request.PageSize, pageCount, totalUserPosts, mappedData);
        }
    }
}
