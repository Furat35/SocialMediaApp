using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Stories;

namespace Posts.Api.Core.Application.Features.Stories.CreateStory
{
    public class CreateStoryCommand : IRequest<ResponseDto<StoryListDto>>
    {
    }
}
