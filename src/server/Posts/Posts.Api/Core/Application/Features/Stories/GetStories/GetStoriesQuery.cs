using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Stories;

namespace Posts.Api.Core.Application.Features.Stories.GetStories
{
    public class GetStoriesQuery : PaginationRequestModel, IRequest<PaginationResponseModel<List<StoryListDto>>>
    {
    }
}
