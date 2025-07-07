using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Friends.DeclineFollow
{
    public class DeclineFollowCommand : IRequest<ResponseDto<bool>>
    {
        public int UserId { get; set; }
    }
}
