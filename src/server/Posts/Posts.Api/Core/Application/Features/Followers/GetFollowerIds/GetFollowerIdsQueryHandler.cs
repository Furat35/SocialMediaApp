using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowers
{
    public class GetFollowerIdsQueryHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<GetFollowerIdsQuery, ResponseDto<List<int>>>
    {
        public async Task<ResponseDto<List<int>>> Handle(GetFollowerIdsQuery request, CancellationToken cancellationToken)
        {
            var followerIds = await followerRepository.Get(_ => (_.RequestingUserId == httpContext.GetUserId() || _.RespondingUserId == httpContext.GetUserId())
                                && _.IsValid && _.Status == FollowStatus.Following)
                .Select(_ => _.RequestingUserId == httpContext.GetUserId() ? _.RespondingUserId : _.RequestingUserId)
                .ToListAsync(cancellationToken);

            return ResponseDto<List<int>>.Success(followerIds, HttpStatusCode.OK);
        }
    }
}
