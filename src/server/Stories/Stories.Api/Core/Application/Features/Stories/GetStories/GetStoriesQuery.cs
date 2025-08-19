using BuildingBlocks.Models;
using MediatR;
using Stories.Api.Core.Application.Dtos.Stories;

namespace Stories.Api.Core.Application.Features.Stories.GetStories
{
    public class GetStoriesQuery : PaginationRequestModel, IRequest<PaginationResponseModel<List<StoryListDto>>>
    {
    }
}
