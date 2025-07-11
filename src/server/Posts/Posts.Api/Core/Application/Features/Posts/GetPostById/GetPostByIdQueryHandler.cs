﻿using AutoMapper;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Enums;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.GetPostById
{
    public class GetPostByIdQueryHandler(IPostRepository postRepository, IMapper mapper, 
        IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<GetPostByIdQuery, ResponseDto<PostListDto>>
    {
        public async Task<ResponseDto<PostListDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.Id, includes: [i => i.Likes, i => i.Comments]);
            if (post == null) return ResponseDto<PostListDto>.Fail("Post not found", HttpStatusCode.NotFound);

            if (await followerRepository.ActiveUserHasAccessToGivenUsersPosts(post.UserId))
            {
                return ResponseDto<PostListDto>.Fail("You do not have permission to access this user's posts.", HttpStatusCode.Forbidden);
            }

            var mappedPost = mapper.Map<PostListDto>(post);

            return ResponseDto<PostListDto>.Success(mappedPost, HttpStatusCode.OK);
        }
    }
}
