using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Followers.RemoveFollowRequest
{
    public class RemoveFollowRequestCommand : IRequest<ResponseDto<bool>>
    {
        public int UserId { get; set; }
    }
}
