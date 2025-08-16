using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Followers.BanFollower
{
    public class BanFollowerCommandHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<BanFollowerCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(BanFollowerCommand request, CancellationToken cancellationToken)
        {
            var follower = await followerRepository
                .Get(_ => ((_.RequestingUserId == request.UserId && _.RespondingUserId == httpContext.GetUserId()) ||
                                   (_.RequestingUserId == httpContext.GetUserId() && _.RespondingUserId == request.UserId)) &&
                                    _.IsValid)
                .FirstOrDefaultAsync();

            if (follower is not null) follower.IsValid = false;

            await followerRepository.AddAsync(
                new Follower
                {
                    RequestingUserId = httpContext.GetUserId(),
                    RespondingUserId = request.UserId,
                    Status = FollowStatus.Banned,
                    IsValid = true
                });

            return ResponseDto<bool>
                .GenerateResponse(await followerRepository.SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail("An error occured while accepting the Follow request", HttpStatusCode.InternalServerError);
        }
    }
}
