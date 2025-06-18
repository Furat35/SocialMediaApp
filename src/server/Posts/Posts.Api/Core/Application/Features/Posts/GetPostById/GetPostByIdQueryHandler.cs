using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.GetPostById
{
    public class GetPostByIdQueryHandler(IPostRepository postRepository, IMapper mapper)
        : IRequestHandler<GetPostByIdQuery, ResponseDto<PostListDto>>
    {
        public async Task<ResponseDto<PostListDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.Id);
            var mappedPost = mapper.Map<PostListDto>(post);

            return ResponseDto<PostListDto>.Success(mappedPost, HttpStatusCode.OK);
        }
    }
}
