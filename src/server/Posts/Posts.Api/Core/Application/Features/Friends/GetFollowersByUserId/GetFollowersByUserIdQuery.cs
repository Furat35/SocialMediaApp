using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Friends;

namespace Posts.Api.Core.Application.Features.Friends.GetFollowersByUserId
{
    public class GetFollowersByUserIdQuery : PaginationRequestModel, IRequest<ResponseDto<PaginationResponseModel<FriendListDto>>>
    {
    }
}
