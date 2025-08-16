using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;
using BuildingBlocks.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowerCount
{
    public class GetFollowerCountQueryHandler(IFollowerRepository followerRepository, 
        IHttpContextAccessor httpContext) 
        : IRequestHandler<GetFollowerCountQuery, int>
    {
        public async Task<int> Handle(GetFollowerCountQuery request, CancellationToken cancellationToken)
        {
            var followerCount = 
                await followerRepository
                .Get(_ => (_.RequestingUserId == httpContext.GetUserId() || 
                    _.RespondingUserId == httpContext.GetUserId())
                    && _.IsValid && _.Status == FollowStatus.Following)
                .CountAsync(cancellationToken);

            return followerCount;
        }
    }
}
