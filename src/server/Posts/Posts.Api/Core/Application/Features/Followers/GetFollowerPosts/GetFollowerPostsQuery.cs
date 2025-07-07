using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Posts;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowerPosts
{
    public class GetFollowerPostsQuery : PaginationRequestModel, IRequest<PaginationResponseModel<PostListDto>>
    {
    }
}
