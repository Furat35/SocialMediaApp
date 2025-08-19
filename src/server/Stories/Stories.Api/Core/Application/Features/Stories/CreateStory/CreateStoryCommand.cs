using BuildingBlocks.Models;
using MediatR;
using Stories.Api.Core.Application.Dtos.Stories;

namespace Stories.Api.Core.Application.Features.Stories.CreateStory
{
    public class CreateStoryCommand : IRequest<ResponseDto<StoryListDto>>
    {
    }
}
