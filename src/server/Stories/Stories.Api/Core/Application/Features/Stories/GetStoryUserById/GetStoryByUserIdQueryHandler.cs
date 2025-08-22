using AutoMapper;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stories.Api.Core.Application.Dtos.Stories;
using Stories.Api.Core.Application.Repositories;
using Stories.Api.ExternalServices;
using System.Net;

namespace Stories.Api.Core.Application.Features.Stories.GetStoryByUserId
{
    public class GetStoryByUserIdQueryHandler(
        IStoryRepository storyRepository,
        IMapper mapper)
        : IRequestHandler<GetStoryByUserIdQuery, ResponseDto<List<StoryListDto>>>
    {
        public async Task<ResponseDto<List<StoryListDto>>> Handle(GetStoryByUserIdQuery request, CancellationToken cancellationToken)
        {
            var story = await storyRepository.Get(_ => _.UserId == request.FollowerId && _.IsValid)
                .OrderByDescending(_ => _.CreateDate)
                .ToListAsync();
            if (!story.Any())
                return ResponseDto<List<StoryListDto>>.Fail(ErrorMessages.NotFound, HttpStatusCode.NotFound);
            if (story[0].UserId != request.FollowerId) return ResponseDto<List<StoryListDto>>.Fail(ErrorMessages.Forbidden, HttpStatusCode.Forbidden);

            return ResponseDto<List<StoryListDto>>.Success(mapper.Map<List<StoryListDto>>(story), HttpStatusCode.OK);
        }
    }
}
