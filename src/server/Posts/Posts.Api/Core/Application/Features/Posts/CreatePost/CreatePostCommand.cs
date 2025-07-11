﻿using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Posts.CreatePost
{
    public class CreatePostCommandRequest : IRequest<ResponseDto<CreatePostCommandResponse>>
    {
        public string Description { get; set; }
        public string Location { get; set; }
        public string Fullname { get; set; }
        public IFormFile File { get; set; }
    }

    public class CreatePostCommandResponse
    {
        public int? Id { get; set; }
        public bool Success { get; set; }
    }
}
