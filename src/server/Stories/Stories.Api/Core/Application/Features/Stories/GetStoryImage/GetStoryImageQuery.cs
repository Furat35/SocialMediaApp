using MediatR;

namespace Stories.Api.Core.Application.Features.Stories.GetStoryImage
{
    public class GetStoryImageQuery : IRequest<(byte[] image, string fileType)>
    {
        public int StoryId { get; set; }
    }
}
