using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Posts;

namespace Posts.Api.Core.Application.Features.Posts.GetPostById
{
    public class GetPostByIdQuery : IRequest<ResponseDto<PostListDto>>
    {
        public int Id { get; set; }
    }
}
