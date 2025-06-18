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
        : IRequestHandler<GetPostsByUserIdQuery, ResponseDto<PaginationResponseModel<List<PostListDto>>>>
    {
        public async Task<ResponseDto<PaginationResponseModel<List<PostListDto>>>> Handle(GetPostsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userPosts = await postRepository
                .GetAll()
                .Where(_ => _.UserId == request.UserId && _.IsValid)
                .ToListAsync(cancellationToken);
            var totalUserPosts = userPosts.Count;
            var pageCount = totalUserPosts / request.PageSize + (totalUserPosts % request.PageSize > 0 ? 1 : 0);
            userPosts = userPosts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            var mappedData = mapper.Map<List<PostListDto>>(userPosts);
            var paginationModel = new PaginationResponseModel<List<PostListDto>>(request.Page, request.PageSize, pageCount, mappedData);

            return ResponseDto<PaginationResponseModel<List<PostListDto>>>.Success(paginationModel, HttpStatusCode.OK);
        }
    }
}
