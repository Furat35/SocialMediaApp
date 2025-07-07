using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Followers.AcceptFollowRequest
{
    public class AcceptFollowRequestCommandHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<AcceptFollowRequestCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(AcceptFollowRequestCommand request, CancellationToken cancellationToken)
        {
            var follow = await followerRepository
                .Get(f => f.RequestingUserId == request.UserId && f.RespondingUserId == httpContext.GetUserId() && f.Status == FollowStatus.Pending)
                .FirstOrDefaultAsync();
            if (follow is null)
                return ResponseDto<bool>.Fail("Follow request does not exist.", HttpStatusCode.BadRequest);

            if (follow.Status == FollowStatus.Accepted)
                return ResponseDto<bool>.Fail("Is already a Follow.", HttpStatusCode.BadRequest);

            follow.IsValid = true;
            follow.Status = FollowStatus.Accepted;

            return ResponseDto<bool>
             .GenerateResponse(await followerRepository.SaveChangesAsync() > 0)
             .Success(true, HttpStatusCode.OK)
             .Fail("An error occured while accepting the Follow request", HttpStatusCode.InternalServerError);
        }
    }
}
