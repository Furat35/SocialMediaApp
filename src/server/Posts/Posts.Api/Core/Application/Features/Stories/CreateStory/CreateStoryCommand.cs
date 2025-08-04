using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Stories.CreateStory
{
    public class CreateStoryCommand : IRequest<ResponseDto<bool>>
    {
    }
}
