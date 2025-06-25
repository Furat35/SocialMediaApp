using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Friends.SendFollowRequestFriend
{
    public class SendFollowRequestCommand : IRequest<ResponseDto<bool>>
    {
        public int UserId { get; set; }
    }
}
