using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Friends.DeclineFollow
{
    public class DeclineFollowCommandHandler(IFriendRepository friendRepository) 
        : IRequestHandler<DeclineFollowCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(DeclineFollowCommand request, CancellationToken cancellationToken)
        {
            var follow = await friendRepository.Get(_ => _.RequestingUserId == request.UserId).FirstOrDefaultAsync(cancellationToken);
            if (follow is null)
                return ResponseDto<bool>.Fail("Follow request does not exist.", HttpStatusCode.BadRequest);

            follow.Status = FriendStatus.Declined;

            return ResponseDto<bool>
             .GenerateResponse(await friendRepository.SaveChangesAsync() > 0)
             .Success(true, HttpStatusCode.OK)
             .Fail("An error occured while sending the Follow request", HttpStatusCode.InternalServerError);
        }
    }
}
