using FluentValidation;
using Posts.Api.Core.Application.Features.Posts.CreatePostComment;
using Posts.Api.Core.Application.Repositories;

namespace Posts.Api.Core.Application.Validators
{
    public class CreatePostCommentCommandValidator : AbstractValidator<CreatePostCommentCommand>
    {
        public CreatePostCommentCommandValidator(IPostRepository postRepository)
        {

        }
    }

}
