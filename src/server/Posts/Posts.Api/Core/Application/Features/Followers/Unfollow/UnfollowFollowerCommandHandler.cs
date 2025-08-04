using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Features.Followers.UnfollowFollower;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Followers.Unfollow
{
    public class UnfollowFollowerCommandHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<UnfollowCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(UnfollowCommand request, CancellationToken cancellationToken)
        {
            var follower = await followerRepository
                .Get(f => (f.RequestingUserId == request.UserId && f.RespondingUserId == httpContext.GetUserId() && f.Status == FollowStatus.Following) ||
                        (f.RespondingUserId == request.UserId && f.RequestingUserId == httpContext.GetUserId()) && f.Status == FollowStatus.Following)
                .FirstOrDefaultAsync();
            if (follower is null)
                return ResponseDto<bool>.Fail("Follow does not exist.", HttpStatusCode.BadRequest);

            follower.IsValid = false;
            return ResponseDto<bool>
              .GenerateResponse(await followerRepository.SaveChangesAsync() > 0)
              .Success(true, HttpStatusCode.OK)
              .Fail("An error occured while deleting the Follow", HttpStatusCode.InternalServerError);
        }
    }
}
