using MediatR;

namespace Posts.Api.Core.Application.Features.Stories.GetStoryImage
{
    public class GetStoryImageQuery : IRequest<(byte[] image, string fileType)>
    {
        public int StoryId { get; set; }
    }
}
