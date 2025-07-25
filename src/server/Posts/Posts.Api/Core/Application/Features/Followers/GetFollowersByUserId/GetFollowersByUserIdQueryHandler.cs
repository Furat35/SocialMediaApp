using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Followers;
using Posts.Api.Core.Application.Repositories;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowersByUserId
{
    public class GetFollowersByUserIdQueryHandler(IFollowerRepository followerRepository)
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
            var pageCount = (int)Math.Ceiling((double)totalFollowers / request.PageSize);

            var responseData = await followers
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginationResponseModel<FollowerListDto>(request.Page, request.PageSize, pageCount, totalFollowers, responseData);

        }
    }
}
