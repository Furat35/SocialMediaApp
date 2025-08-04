using MediatR;

namespace Posts.Api.Core.Application.Features.Stories.DeleteStory
{
    public class DeleteStoryCommandHandler : IRequestHandler<DeleteStoryCommand, bool>
    {
        public async Task<bool> Handle(DeleteStoryCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
