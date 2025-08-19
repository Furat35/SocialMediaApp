using AutoMapper;
using IdentityServer.Api.Business.Dtos.Followers;
using IdentityServer.Api.Core.Domain.Entities;

namespace IdentityServer.Api.Core.Application.Mappings
{
    public class EntityMappings : Profile
    {
        public EntityMappings()
        {
            CreateMap<Follower, FollowerListDto>();
        }
    }
}
