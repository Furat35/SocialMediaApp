using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
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
                .Get(f => f.RequestingUserId == httpContextAccessor.GetUserId() && f.RespondingUserId == request.UserId)
                .AnyAsync();
            if (followExists)
                return ResponseDto<bool>.Fail("Follow request already exists.", HttpStatusCode.BadRequest);

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
    }
}
