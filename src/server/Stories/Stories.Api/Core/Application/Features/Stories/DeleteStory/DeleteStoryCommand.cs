using BuildingBlocks.Models;
using MediatR;

namespace Stories.Api.Core.Application.Features.Stories.DeleteStory
{
    public class DeleteStoryCommand : IRequest<ResponseDto<bool>>
    {
        public int StoryId { get; set; }
    }
}
