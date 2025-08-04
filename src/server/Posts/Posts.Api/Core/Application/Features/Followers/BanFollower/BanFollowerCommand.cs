using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Followers.BanFollower
{
    public class BanFollowerCommand : IRequest<ResponseDto<bool>>
    {
        public int UserId { get; set; }
    }
}
