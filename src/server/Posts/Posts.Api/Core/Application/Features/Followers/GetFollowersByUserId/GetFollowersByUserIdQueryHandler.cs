using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Followers;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowersByUserId
{
    public class GetFollowersByUserIdQueryHandler(IFollowerRepository followerRepository, IHttpContextAccessor httpContext, IMapper mapper)
        : IRequestHandler<GetFollowersByUserIdQuery, PaginationResponseModel<FollowerListDto>>
    {
        public async Task<PaginationResponseModel<FollowerListDto>> Handle(GetFollowersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var followers = followerRepository
               .Get(_ => (_.RequestingUserId == request.UserId || _.RespondingUserId == request.UserId) && _.Status == request.Status)
               .Select(_ => new FollowerListDto
               {
                   Id = _.Id,
                   RequestingUserId = _.RequestingUserId,
                   RespondingUserId = _.RespondingUserId,
                   Status = _.Status,
                   CreateDate = _.CreateDate
               });

            var totalFollowers = await followers.CountAsync();
            var pageCount = totalFollowers / request.PageSize + (totalFollowers % request.PageSize > 0 ? 1 : 0);

            var responseData = await followers
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginationResponseModel<FollowerListDto>(request.Page, request.PageSize, pageCount, totalFollowers, responseData);

        }
    }
}
