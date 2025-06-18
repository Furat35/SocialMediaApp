using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Posts;

namespace Posts.Api.Core.Application.Features.Posts.GetPostsByUserId
{
    public class GetPostsByUserIdQuery : PaginationRequestModel, IRequest<ResponseDto<PaginationResponseModel<List<PostListDto>>>>
    {
        public int UserId { get; set; }
    }
}
