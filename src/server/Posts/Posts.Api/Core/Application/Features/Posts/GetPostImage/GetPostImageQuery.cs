using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Posts.GetPostImage
{
    public class GetPostImageQuery : IRequest<(byte[] image, string fileType)>
    {
        public int PostId { get; set; }
    }
}
