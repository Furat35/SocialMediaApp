using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Posts;

namespace Posts.Api.Core.Application.Features.Friends.GetFollowerPosts
{
    public class GetFollowerPostsQuery : PaginationRequestModel, IRequest<ResponseDto<PaginationResponseModel<PostListDto>>>
    {
    }
}
