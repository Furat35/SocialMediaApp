using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Posts.UnlikePost
{
    public class UnlikePostCommand : IRequest<ResponseDto<bool>>
    {
        public int PostId { get; set; }
    }
}
