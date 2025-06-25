using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Posts.LikePost
{
    public class LikePostCommand : IRequest<ResponseDto<bool>>
    {
        public int PostId { get; set; }
    }
}
