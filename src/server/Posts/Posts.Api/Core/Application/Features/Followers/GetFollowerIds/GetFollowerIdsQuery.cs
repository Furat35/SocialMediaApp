using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowers
{
    public class GetFollowerIdsQuery : IRequest<ResponseDto<List<int>>>
    {
    }
}
