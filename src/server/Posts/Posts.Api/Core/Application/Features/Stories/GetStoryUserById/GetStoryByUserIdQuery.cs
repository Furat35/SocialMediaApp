using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Stories;

namespace Posts.Api.Core.Application.Features.Stories.GetStoryByUserId
{
    public class GetStoryByUserIdQuery : IRequest<ResponseDto<List<StoryListDto>>>
    {
        public int UserId { get; set; }
    }
}
