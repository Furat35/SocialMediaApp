using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using BuildingBlocks.Extensions;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Models;
using System.Net;

namespace Posts.Api.Core.Application.Features.Followers.RemoveBan
{
    public class RemoveBanCommandHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext) 
        : IRequestHandler<RemoveBanCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(RemoveBanCommand request, CancellationToken cancellationToken)
        {
            var follower = await followerRepository
              .Get(_ => (_.RequestingUserId == httpContext.GetUserId() && _.RespondingUserId == request.UserId) 
                    && _.IsValid && _.Status == FollowStatus.Banned)
              .FirstOrDefaultAsync();

            if (follower is null)
                return ResponseDto<bool>.Fail("Ban does not exist.", HttpStatusCode.BadRequest);

            follower.IsValid = false;
            return ResponseDto<bool>
                .GenerateResponse(await followerRepository.SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail("An error occured while accepting the Follow request", HttpStatusCode.InternalServerError);
        }
    }
}
