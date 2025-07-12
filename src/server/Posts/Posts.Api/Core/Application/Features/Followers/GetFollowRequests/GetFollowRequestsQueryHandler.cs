using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Followers;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowRequests
{
    public class GetFollowRequestsQueryHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        : IRequestHandler<GetFollowRequestsQuery, PaginationResponseModel<FollowerListDto>>
    {
        public async Task<PaginationResponseModel<FollowerListDto>> Handle(GetFollowRequestsQuery request, CancellationToken cancellationToken)
        {
            var followerResponse = followerRepository
                .Get(_ => _.Status == FollowStatus.Pending && _.RespondingUserId == httpContextAccessor.GetUserId())
                .Select(_ => new FollowerListDto
                {
                    Id = _.Id,
                    RequestingUserId = _.RequestingUserId,
                    RespondingUserId = _.RespondingUserId,
                    Status = _.Status,
                    CreateDate = _.CreateDate
                });

            var totalfollowRequests = await followerResponse.CountAsync();
            var pageCount = totalfollowRequests / request.PageSize + (totalfollowRequests % request.PageSize > 0 ? 1 : 0);

            var response = await followerResponse
                .OrderByDescending(_ => _.CreateDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mappedData = mapper.Map<List<FollowerListDto>>(response);
            var paginationModel = new PaginationResponseModel<FollowerListDto>(request.Page, request.PageSize, pageCount, totalfollowRequests, mappedData);

            return paginationModel;
        }
    }
}
