using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Followers.CancelFollowRequest
{
    public class CancelFollowRequestCommandHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext) : IRequestHandler<CancelFollowRequestCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(CancelFollowRequestCommand request, CancellationToken cancellationToken)
        {
            var follow = await followerRepository
                .Get(_ => _.RequestingUserId == httpContext.GetUserId() && _.RespondingUserId == request.UserId && 
                     _.Status == FollowStatus.Pending && _.IsValid)
                .FirstOrDefaultAsync(cancellationToken);
            if (follow is null)
                return ResponseDto<bool>.Fail("Follow request does not exist.", HttpStatusCode.BadRequest);

            follow.Status = FollowStatus.Cancelled;
            follow.IsValid = false;

            return ResponseDto<bool>
                .GenerateResponse(await followerRepository.SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail("An error occured while canceling the Follow request", HttpStatusCode.InternalServerError);
        }
    }
}
