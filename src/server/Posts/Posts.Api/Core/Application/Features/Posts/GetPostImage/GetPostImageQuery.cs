using MediatR;
using Posts.Api.Behaviours;

namespace Posts.Api.Core.Application.Features.Posts.GetPostImage
{
    public class GetPostImageQuery : IRequest<(byte[] image, string fileType)>, IRequiresFollowCheck
    {
        public int PostId { get; set; }

        public int FollowerId { get; set; }
    }
}
