using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Followers.DeclineFollow
{
    public class DeclineFollowCommandHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<DeclineFollowCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(DeclineFollowCommand request, CancellationToken cancellationToken)
        {
            var follow = await followerRepository
                .Get(_ => _.RequestingUserId == request.UserId && _.RespondingUserId == httpContext.GetUserId() &&
                    _.Status == FollowStatus.Pending && _.IsValid)
                .FirstOrDefaultAsync(cancellationToken);
            if (follow is null)
                return ResponseDto<bool>.Fail("Follow request does not exist.", HttpStatusCode.BadRequest);

            follow.Status = FollowStatus.Declined;
            follow.IsValid = false;

            return ResponseDto<bool>
             .GenerateResponse(await followerRepository.SaveChangesAsync() > 0)
             .Success(true, HttpStatusCode.OK)
             .Fail("An error occured while sending the Follow request", HttpStatusCode.InternalServerError);
        }
    }
}
