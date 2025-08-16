using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Stories.DeleteStory
{
    public class DeleteStoryCommand : IRequest<ResponseDto<bool>>
    {
        public int StoryId { get; set; }
    }
}
