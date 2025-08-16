using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Dtos.Stories;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Stories.GetStoryByUserId
{
    public class GetStoryByUserIdQueryHandler(IStoryRepository storyRepository, IFollowerRepository followerRepository,
        IMapper mapper)
        : IRequestHandler<GetStoryByUserIdQuery, ResponseDto<List<StoryListDto>>>
    {
        public async Task<ResponseDto<List<StoryListDto>>> Handle(GetStoryByUserIdQuery request, CancellationToken cancellationToken)
        {
            var story = await storyRepository.Get(_ => _.UserId == request.UserId && _.IsValid)
                .OrderByDescending(_ => _.CreateDate)
                .ToListAsync();
            if (!story.Any())
                return ResponseDto<List<StoryListDto>>.Fail("Story not found", HttpStatusCode.NotFound);
            if (!await followerRepository.ActiveUserHasAccessToGivenUser(story[0].UserId))
                return ResponseDto<List<StoryListDto>>.Fail("You do not have permission to access this user's stories.", HttpStatusCode.Forbidden);

            return ResponseDto<List<StoryListDto>>.Success(mapper.Map<List<StoryListDto>>(story), HttpStatusCode.OK);
        }
    }
}
