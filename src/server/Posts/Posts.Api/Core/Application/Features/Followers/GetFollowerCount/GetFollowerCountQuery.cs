using MediatR;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowerCount
{
    public class GetFollowerCountQuery  : IRequest<int>
    {
    }
}
