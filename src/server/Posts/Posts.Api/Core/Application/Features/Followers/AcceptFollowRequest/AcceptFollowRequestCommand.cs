using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Followers.AcceptFollowRequest
{
    public class AcceptFollowRequestCommand : IRequest<ResponseDto<bool>>
    {
        public int UserId { get; set; }
    }
}
