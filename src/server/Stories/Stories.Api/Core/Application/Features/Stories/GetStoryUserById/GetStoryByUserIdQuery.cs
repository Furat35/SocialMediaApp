using BuildingBlocks.Models;
using MediatR;
using Stories.Api.Behaviours;
using Stories.Api.Core.Application.Dtos.Stories;

namespace Stories.Api.Core.Application.Features.Stories.GetStoryByUserId
{
    public class GetStoryByUserIdQuery : IRequest<ResponseDto<List<StoryListDto>>>, IRequiresFollowCheck
    {
        public int UserId { get; set; }
        public int FollowerId => UserId;

    }
}
