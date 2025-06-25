using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Friends.AcceptFollowRequest
{
    public class AcceptFollowRequestCommandHandler(IFriendRepository FollowRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<AcceptFollowRequestCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(AcceptFollowRequestCommand request, CancellationToken cancellationToken)
        {
            var Follow = await FollowRepository
                .Get(f => f.RequestingUserId == request.UserId && f.RespondingUserId == httpContext.GetUserId())
                .FirstOrDefaultAsync();
            if (Follow is null)
                return ResponseDto<bool>.Fail("Follow request does not exist.", System.Net.HttpStatusCode.BadRequest);

            if (Follow.IsValid)
                return ResponseDto<bool>.Fail("Is already a Follow.", System.Net.HttpStatusCode.BadRequest);

            Follow.IsValid = true;

            return ResponseDto<bool>
             .GenerateResponse(await FollowRepository.SaveChangesAsync() > 0)
             .Success(true, HttpStatusCode.OK)
             .Fail("An error occured while accepting the Follow request", HttpStatusCode.InternalServerError);
        }
    }
}
