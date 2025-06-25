using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.GetPostsByUserId
{
    public class GetPostsByUserIdQueryHandler(IPostRepository postRepository, IMapper mapper)
        : IRequestHandler<GetPostsByUserIdQuery, ResponseDto<PaginationResponseModel<PostListDto>>>
    {
        public async Task<ResponseDto<PaginationResponseModel<PostListDto>>> Handle(GetPostsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userPosts = postRepository
                .Get(_ => _.UserId == request.UserId && _.IsValid);

            var totalUserPosts = await userPosts.CountAsync();
            var pageCount = totalUserPosts / request.PageSize + (totalUserPosts % request.PageSize > 0 ? 1 : 0);

            var response = await userPosts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mappedData = mapper.Map<List<PostListDto>>(response);
            var paginationModel = new PaginationResponseModel<PostListDto>(request.Page, request.PageSize, pageCount, mappedData);

            return ResponseDto<PaginationResponseModel<PostListDto>>.Success(paginationModel, HttpStatusCode.OK);
        }
    }
}
