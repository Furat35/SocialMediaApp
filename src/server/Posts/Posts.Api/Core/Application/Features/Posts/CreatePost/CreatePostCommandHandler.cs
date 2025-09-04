using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Interfaces.Services;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Core.Application.Features.Posts.CreatePost
{
    public class CreatePostCommandHandler(
        IPostRepository postRepository,
        IMapper mapper,
        IHttpContextAccessor httpContext,
        IFileService fileService)
        : BaseHandler<IPostRepository, Post>(postRepository),
            IRequestHandler<CreatePostCommandRequest, ResponseDto<CreatePostCommandResponse>>
    {
        public async Task<ResponseDto<CreatePostCommandResponse>> Handle(CreatePostCommandRequest request, CancellationToken cancellationToken)
        {
            var post = mapper.Map<Post>(request);
            int result;
            string path = null;
            post.UserId = httpContext.GetUserId();
            try
            {
                var file = httpContext.GetFirstFormFile();
                path = await fileService.SaveFileAsync(file, "images/users/posts");
                post.ImagePath = path;
                await _repository.AddAsync(post);
                result = await _repository.SaveChangesAsync();
            }
            catch
            {
                fileService.RemoveFile(path);
                return ReturnFail<CreatePostCommandResponse>(ErrorMessages.SaveChangesError);
            }

            var response = new CreatePostCommandResponse { Id = post?.Id, Success = result > 0 };

            return ReturnSuccess(response);
        }
    }
}
