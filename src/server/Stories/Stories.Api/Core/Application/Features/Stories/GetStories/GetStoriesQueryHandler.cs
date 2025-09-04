using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stories.Api.Core.Application.Dtos.Stories;
using Stories.Api.Core.Application.Repositories;
using Stories.Api.Core.Domain.Entities;
using Stories.Api.ExternalServices;

namespace Stories.Api.Core.Application.Features.Stories.GetStories
{
    public class GetStoriesQueryHandler(
        IStoryRepository storyRepository,
        IMapper mapper,
        IFollowerService followerService)
        : BaseHandler<IStoryRepository, Story>(storyRepository),
            IRequestHandler<GetStoriesQuery, PaginationResponseModel<List<StoryListDto>>>
    {
        public async Task<PaginationResponseModel<List<StoryListDto>>> Handle(GetStoriesQuery request, CancellationToken cancellationToken)
        {
            var followerIds = await followerService.GetFollowerIdsAsync();

            var storyFollowerIds = storyRepository
                .Get(_ => followerIds.Contains(_.UserId) && _.IsValid)
                .OrderByDescending(_ => _.CreateDate)
                .Select(_ => _.UserId)
                .Distinct();

            var totalStories = await storyFollowerIds.CountAsync();
            var pageCount = (int)Math.Ceiling((double)totalStories / request.PageSize);

            var res = await storyFollowerIds
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var stories = new List<List<StoryListDto>>();
            foreach (var followerId in res)
            {
                var followerStories = await storyRepository
                    .Get(_ => followerId == _.UserId && _.IsValid)
                    .OrderByDescending(_ => _.CreateDate)
                    .ToListAsync(cancellationToken);
                stories.Add(mapper.Map<List<StoryListDto>>(followerStories));
            }

            return new PaginationResponseModel<List<StoryListDto>>(request.Page, request.PageSize, pageCount,
                totalStories, mapper.Map<List<List<StoryListDto>>>(stories));
        }
    }
}
