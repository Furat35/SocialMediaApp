using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Friends.RemoveFollowRequest
{
    public class RemoveFollowRequestCommandHandler(IFriendRepository FollowRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<RemoveFollowRequestCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(RemoveFollowRequestCommand request, CancellationToken cancellationToken)
        {
            var Follow = await FollowRepository
                .Get(f => (f.RequestingUserId == request.UserId && f.RespondingUserId == httpContext.GetUserId() && f.IsValid) ||
                        (f.RespondingUserId == request.UserId && f.RequestingUserId == httpContext.GetUserId()) && f.IsValid)
                .FirstOrDefaultAsync();
            if (Follow is null)
                return ResponseDto<bool>.Fail("Follow does not exist.", HttpStatusCode.BadRequest);

            FollowRepository.Remove(Follow);
            return ResponseDto<bool>
              .GenerateResponse(await FollowRepository.SaveChangesAsync() > 0)
              .Success(true, HttpStatusCode.OK)
              .Fail("An error occured while deleting the Follow", HttpStatusCode.InternalServerError);
        }
    }
}
