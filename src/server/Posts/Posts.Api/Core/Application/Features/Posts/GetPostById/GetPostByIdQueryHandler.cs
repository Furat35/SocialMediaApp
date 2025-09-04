using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.GetPostById
{
    public class GetPostByIdQueryHandler(
        IPostRepository postRepository,
        IMapper mapper)
        : BaseHandler<IPostRepository, Post>(postRepository),
            IRequestHandler<GetPostByIdQuery, ResponseDto<PostListDto>>
    {
        public async Task<ResponseDto<PostListDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var post = await _repository.GetFirstAsync(
                _ => _.Id == request.Id && _.UserId == request.FollowerId && _.IsValid,
                [i => i.Likes, i => i.Comments]);
            if (post is null) return ToResponse<PostListDto>(HttpStatusCode.NotFound);

            var mappedPost = mapper.Map<PostListDto>(post);

            return ReturnSuccess(mappedPost);
        }
    }
}
