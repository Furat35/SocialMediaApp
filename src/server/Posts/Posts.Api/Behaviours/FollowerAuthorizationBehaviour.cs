using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Posts.Api.ExternalServices;
using System.Net;

namespace Posts.Api.Behaviours
{
    public class FollowerAuthorizationBehavior<TRequest, TResponse>(IFollowerService followerService)
      : IPipelineBehavior<TRequest, TResponse>
      where TRequest : class
      where TResponse : class
    {

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is IRequiresFollowCheck followRequest)
            {
                var isFollowing = await followerService.IsFollowing(followRequest.FollowerId);
                if (!isFollowing)
                {
                    return (ResponseDto<object>.Fail(null, ErrorMessages.Forbidden, HttpStatusCode.Forbidden) as TResponse)!;
                }
            }

            return await next();
        }
    }

    public interface IRequiresFollowCheck
    {
        int FollowerId { get; }
    }
}
