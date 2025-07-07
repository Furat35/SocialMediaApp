using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Friends;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;

namespace Posts.Api.Core.Application.Features.Friends.GetFollowRequests
{
    public class GetFollowRequestsQueryHandler(IFriendRepository friendRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        : IRequestHandler<GetFollowRequestsQuery, PaginationResponseModel<GetFollowRequestsResponseQuery>>
    {
        public async Task<PaginationResponseModel<GetFollowRequestsResponseQuery>> Handle(GetFollowRequestsQuery request, CancellationToken cancellationToken)
        {
            var friendResponse = friendRepository
                .Get(_ => _.Status == FriendStatus.Pending && _.RespondingUserId == httpContextAccessor.GetUserId())
                .Select(_ => new GetFollowRequestsResponseQuery
                {
                     Id = _.Id,
                     RequestingUserId = _.RequestingUserId,
                     CreateDate = _.CreateDate,
                });

            var totalfriendRequests = await friendResponse.CountAsync();
            var pageCount = totalfriendRequests / request.PageSize + (totalfriendRequests % request.PageSize > 0 ? 1 : 0);

            var response = await friendResponse
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mappedData = mapper.Map<List<GetFollowRequestsResponseQuery>>(response);
            var paginationModel = new PaginationResponseModel<GetFollowRequestsResponseQuery>(request.Page, request.PageSize, pageCount, totalfriendRequests, mappedData);

            return paginationModel;
        }
    }
}
