using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Posts.UpdatePost
{
    public class UpdatePostCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
    }
}
