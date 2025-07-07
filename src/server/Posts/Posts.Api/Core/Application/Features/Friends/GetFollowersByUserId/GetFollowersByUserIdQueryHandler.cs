using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Friends;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Friends.GetFollowersByUserId
{
    public class GetFollowersByUserIdQueryHandler(IFriendRepository friendRepository, IHttpContextAccessor httpContext, IMapper mapper)
        : IRequestHandler<GetFollowersByUserIdQuery, ResponseDto<PaginationResponseModel<FriendListDto>>>
    {
        public async Task<ResponseDto<PaginationResponseModel<FriendListDto>>> Handle(GetFollowersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var friends = friendRepository
               .Get(_ => (_.RequestingUserId == request.UserId || _.RespondingUserId == request.UserId) && _.Status == FriendStatus.Accepted)
               .Select(_ => new FriendListDto
               {
                   UserId = _.RequestingUserId == request.UserId ? _.RespondingUserId : _.RequestingUserId,
                   CreateDate = _.CreateDate
               });

            var totalFriends = await friends.CountAsync();
            var pageCount = totalFriends / request.PageSize + (totalFriends % request.PageSize > 0 ? 1 : 0);

            var responseData = await friends
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var paginationModel = new PaginationResponseModel<FriendListDto>(request.Page, request.PageSize, pageCount, totalFriends, responseData);

            return ResponseDto<PaginationResponseModel<FriendListDto>>.Success(paginationModel, HttpStatusCode.OK);
        }
    }
}
