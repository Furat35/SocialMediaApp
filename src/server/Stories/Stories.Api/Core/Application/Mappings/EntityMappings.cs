using AutoMapper;
using Stories.Api.Core.Application.Dtos.Stories;
using Stories.Api.Core.Domain.Entities;

namespace Stories.Api.Core.Application.Mappings
{
    public class EntityMappings : Profile
    {
        public EntityMappings()
        {
            CreateMap<Story, StoryListDto>();
        }
    }
}
