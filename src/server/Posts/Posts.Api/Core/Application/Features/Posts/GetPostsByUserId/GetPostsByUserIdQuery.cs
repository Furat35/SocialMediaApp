using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Posts;

namespace Posts.Api.Core.Application.Features.Posts.GetPostsByUserId
{
    public class GetPostsByUserIdQuery : PaginationRequestModel, IRequest<ResponseDto<PaginationResponseModel<PostListDto>>>
    {
        public int UserId { get; set; }
    }
}
