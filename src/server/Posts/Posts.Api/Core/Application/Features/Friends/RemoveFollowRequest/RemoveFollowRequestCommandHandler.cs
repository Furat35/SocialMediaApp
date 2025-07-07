using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Friends.RemoveFollowRequest
{
    public class RemoveFollowRequestCommandHandler(IFriendRepository followRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<RemoveFollowRequestCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(RemoveFollowRequestCommand request, CancellationToken cancellationToken)
        {
            var follow = await followRepository
                .Get(f => (f.RequestingUserId == request.UserId && f.RespondingUserId == httpContext.GetUserId() && f.Status == FriendStatus.Accepted) ||
                        (f.RespondingUserId == request.UserId && f.RequestingUserId == httpContext.GetUserId()) && f.Status == FriendStatus.Accepted)
                .FirstOrDefaultAsync();
            if (follow is null)
                return ResponseDto<bool>.Fail("Follow does not exist.", HttpStatusCode.BadRequest);

            followRepository.Remove(follow);
            return ResponseDto<bool>
              .GenerateResponse(await followRepository.SaveChangesAsync() > 0)
              .Success(true, HttpStatusCode.OK)
              .Fail("An error occured while deleting the Follow", HttpStatusCode.InternalServerError);
        }
    }
}
