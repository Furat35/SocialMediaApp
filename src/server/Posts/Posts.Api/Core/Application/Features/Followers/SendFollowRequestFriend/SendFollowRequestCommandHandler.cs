using Azure.Core;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Followers.SendFollowRequestFriend
{
    public class SendFollowRequestCommandHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<SendFollowRequestCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(SendFollowRequestCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == httpContextAccessor.GetUserId())
                return ResponseDto<bool>.Fail("You cannot send a Follow request to yourself.", HttpStatusCode.BadRequest);
            var followExists = await followerRepository
                .Get(f => (f.RequestingUserId == httpContextAccessor.GetUserId() && f.RespondingUserId == request.UserId) ||
                          (f.RequestingUserId == request.UserId && f.RespondingUserId == httpContextAccessor.GetUserId()))
                .OrderByDescending(_ => _.CreateDate)
                .FirstOrDefaultAsync();

            var result = await CheckFollowStatus(followExists);
            if(!result.isValid) return ResponseDto<bool>.Fail(result.message, HttpStatusCode.BadRequest);

            await followerRepository.AddAsync(new Follower
            {
                RequestingUserId = httpContextAccessor.GetUserId(),
                RespondingUserId = request.UserId
            });

            return ResponseDto<bool>
                .GenerateResponse(await followerRepository.SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.Created)
                .Fail("An error occured while sending the Follow request", HttpStatusCode.InternalServerError);
        }

        private async Task<(bool isValid, string? message)> CheckFollowStatus(Follower follower)
        {
            if (follower is not null)
            {
                var message = string.Empty;
                switch (follower.Status)
                {
                    case FollowStatus.Pending:
                        message = "Follow request already exists."; break;
                    case FollowStatus.Following:
                        message = "Already following."; break;
                    case FollowStatus.Declined:
                        if (follower.CreateDate.AddDays(7) > DateTime.UtcNow)
                            message = $"You cannot send a Follow request to this user within {(follower.CreateDate.AddDays(7) - DateTime.UtcNow).Days} days of declining the previous request.";
                        else return (true, null);
                        break;
                }
                return (false, message);
            }

            return (true, null);
        }
    }
}
