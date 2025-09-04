using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stories.Api.Core.Application.Dtos.Stories;
using Stories.Api.Core.Application.Repositories;
using Stories.Api.Core.Domain.Entities;

namespace Stories.Api.Core.Application.Features.Stories.GetStoryByUserId
{
    public class GetStoryByUserIdQueryHandler(
        IStoryRepository storyRepository,
        IMapper mapper)
        : BaseHandler<IStoryRepository, Story>(storyRepository),
            IRequestHandler<GetStoryByUserIdQuery, ResponseDto<List<StoryListDto>>>
    {
        public async Task<ResponseDto<List<StoryListDto>>> Handle(GetStoryByUserIdQuery request, CancellationToken cancellationToken)
        {
            var story = await storyRepository
                .Get(_ => _.UserId == request.UserId && _.IsValid)
                .OrderByDescending(_ => _.CreateDate)
                .ToListAsync();

            return ReturnSuccess(mapper.Map<List<StoryListDto>>(story));
        }
    }
}
