using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.CreatePost
{
    public class CreatePostCommandHandler(IPostRepository postRepository, IMapper mapper) : IRequestHandler<CreatePostCommandRequest, ResponseDto<CreatePostCommandResponse>>
    {
        public async Task<ResponseDto<CreatePostCommandResponse>> Handle(CreatePostCommandRequest request, CancellationToken cancellationToken)
        {
            var post = mapper.Map<Domain.Entities.Post>(request);
            await postRepository.AddAsync(post);
            var result = await postRepository.SaveChangesAsync();

            var response = new CreatePostCommandResponse
            {
                Id = post?.Id,
                Success = result > 0
            };

            return ResponseDto<CreatePostCommandResponse>.Success(response, HttpStatusCode.OK);
        }
    }
}
