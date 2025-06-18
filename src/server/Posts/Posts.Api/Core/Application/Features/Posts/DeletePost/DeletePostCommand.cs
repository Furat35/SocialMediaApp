using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Posts.DeletePost
{
    public class DeletePostCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
    }
}
