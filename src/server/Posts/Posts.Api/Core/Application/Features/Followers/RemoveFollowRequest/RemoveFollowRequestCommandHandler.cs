using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Followers.RemoveFollowRequest
{
    public class RemoveFollowRequestCommandHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<RemoveFollowRequestCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(RemoveFollowRequestCommand request, CancellationToken cancellationToken)
        {
            var follower = await followerRepository
                .Get(f => (f.RequestingUserId == request.UserId && f.RespondingUserId == httpContext.GetUserId() && f.Status == FollowStatus.Accepted) ||
                        (f.RespondingUserId == request.UserId && f.RequestingUserId == httpContext.GetUserId()) && f.Status == FollowStatus.Accepted)
                .FirstOrDefaultAsync();
            if (follower is null)
                return ResponseDto<bool>.Fail("Follow does not exist.", HttpStatusCode.BadRequest);

            followerRepository.Remove(follower);
            return ResponseDto<bool>
              .GenerateResponse(await followerRepository.SaveChangesAsync() > 0)
              .Success(true, HttpStatusCode.OK)
              .Fail("An error occured while deleting the Follow", HttpStatusCode.InternalServerError);
        }
    }
}
