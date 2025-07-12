using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Followers.UnfollowFollower
{
    public class UnfollowCommand : IRequest<ResponseDto<bool>>
    {
        public int UserId { get; set; }
    }
}
