using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Followers;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowersByUserIds
{
    public class GetFollowersByUserIdsQueryHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext, IMapper mapper) : IRequestHandler<GetFollowersByUserIdsQuery, PaginationResponseModel<FollowerListDto>>
    {
        public async Task<PaginationResponseModel<FollowerListDto>> Handle(GetFollowersByUserIdsQuery request, CancellationToken cancellationToken)
        {
            var userIds = request.UserIds.Distinct().Except([httpContext.GetUserId()]).ToList();
            var x = httpContext.GetUserId();
            var followers = followerRepository
                        .Get(_ => ((userIds.Contains(_.RequestingUserId) && _.RespondingUserId == httpContext.GetUserId())
                        || (userIds.Contains(_.RespondingUserId) && _.RequestingUserId == httpContext.GetUserId()))
                        && _.Status == FollowStatus.Following)
                        .Select(_ => new FollowerListDto
                        {
                            Id = _.Id,
                            RequestingUserId = _.RequestingUserId,
                            RespondingUserId = _.RespondingUserId,
                            Status = _.Status,
                            CreateDate = _.CreateDate
                        });

            var totalFollowers = await followers.CountAsync();
            var pageCount = (int)Math.Ceiling((double)totalFollowers / request.PageSize);

            var responseData = await followers
                .OrderByDescending(_ => _.CreateDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginationResponseModel<FollowerListDto>(request.Page, request.PageSize, pageCount, totalFollowers, responseData);
        }
    }
}
