using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Interfaces.Services;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.CreatePost
{
    public class CreatePostCommandHandler(
        IPostRepository postRepository,
        IMapper mapper,
        IHttpContextAccessor httpContext,
        IFileService fileService)
        : IRequestHandler<CreatePostCommandRequest, ResponseDto<CreatePostCommandResponse>>
    {
        public async Task<ResponseDto<CreatePostCommandResponse>> Handle(CreatePostCommandRequest request, CancellationToken cancellationToken)
        {
            var post = mapper.Map<Domain.Entities.Post>(request);
            int result;
            string path = null;
            post.UserId = httpContext.GetUserId();
            try
            {
                var file = httpContext.HttpContext.Request.Form.Files.FirstOrDefault();
                path = await fileService.SaveFileAsync(file, "images/users/posts");
                post.ImagePath = path;
                await postRepository.AddAsync(post);
                result = await postRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                fileService.RemoveFile(path);
                throw;
            }

            var response = new CreatePostCommandResponse
            {
                Id = post?.Id,
                Success = result > 0
            };

            return ResponseDto<CreatePostCommandResponse>.Success(response, HttpStatusCode.OK);
        }
    }
}
