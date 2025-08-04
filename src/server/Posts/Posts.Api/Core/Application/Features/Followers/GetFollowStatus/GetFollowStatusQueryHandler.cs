using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Followers;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowStatus
{
    public class GetFollowStatusQueryHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<GetFollowStatusQuery, ResponseDto<FollowerListDto>>
    {
        public async Task<ResponseDto<FollowerListDto>> Handle(GetFollowStatusQuery request, CancellationToken cancellationToken)
        {
            var follower = await followerRepository
              .Get(_ => ((_.RequestingUserId == request.UserId && _.RespondingUserId == httpContext.GetUserId()) ||
                    (_.RequestingUserId == httpContext.GetUserId() && _.RespondingUserId == request.UserId)) &&
                     _.IsValid)
              .OrderByDescending(_ => _.CreateDate)
              .Select(_ => new FollowerListDto
              {
                  Id = _.Id,
                  RequestingUserId = _.RequestingUserId,
                  RespondingUserId = _.RespondingUserId,
                  Status = _.Status,
                  CreateDate = _.CreateDate
              }).FirstOrDefaultAsync();

            follower ??= new FollowerListDto
            {
                Id = null,
                RequestingUserId = null,
                RespondingUserId = null,
                Status = FollowStatus.NotFollowing,
                CreateDate = null
            };

            return ResponseDto<FollowerListDto>.Success(follower, HttpStatusCode.OK);

        }
    }
}
