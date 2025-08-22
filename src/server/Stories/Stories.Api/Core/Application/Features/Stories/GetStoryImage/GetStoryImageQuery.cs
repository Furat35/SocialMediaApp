using MediatR;
using Stories.Api.Behaviours;

namespace Stories.Api.Core.Application.Features.Stories.GetStoryImage
{
    public class GetStoryImageQuery : IRequest<(byte[] image, string fileType)>, IRequiresFollowCheck
    {
        public int StoryId { get; set; }
        public int FollowerId { get; set; }
    }
}
