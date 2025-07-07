using AutoMapper;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Features.Posts.CreatePost;
using Posts.Api.Core.Application.Features.Posts.CreatePostComment;
using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Core.Application.Mappings
{
    public class EntityMappings : Profile
    {
        public EntityMappings()
        {
            CreateMap<CreatePostCommandRequest, Post>();
            CreateMap<CreatePostCommentCommand, Comment>();
            CreateMap<Post, PostListDto>();


            CreateMap<Like, LikeListDto>();
            CreateMap<Comment, CommentListDto>();
        }
    }
}
