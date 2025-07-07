using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Followers.DeclineFollow
{
    public class DeclineFollowCommand : IRequest<ResponseDto<bool>>
    {
        public int UserId { get; set; }
    }
}
