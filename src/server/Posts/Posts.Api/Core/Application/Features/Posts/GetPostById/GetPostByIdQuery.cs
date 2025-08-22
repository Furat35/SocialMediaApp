using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Behaviours;
using Posts.Api.Core.Application.Dtos.Posts;

namespace Posts.Api.Core.Application.Features.Posts.GetPostById
{
    public class GetPostByIdQuery : IRequest<ResponseDto<PostListDto>>, IRequiresFollowCheck
    {
        public int Id { get; set; }

        public int FollowerId { get; set; }
    }
}
