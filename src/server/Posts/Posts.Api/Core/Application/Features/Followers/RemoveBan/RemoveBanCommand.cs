using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Followers.RemoveBan
{
    public class RemoveBanCommand : IRequest<ResponseDto<bool>>
    {
        public int UserId { get; set; }
    }
}
