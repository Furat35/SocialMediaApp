using FluentValidation;
using MediatR;

namespace Posts.Api.Behaviours
{
    //public class ValidationBehavior
    //{
    //}

    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
      : IPipelineBehavior<TRequest, TResponse>
      where TRequest : class
      where TResponse : class, new()
    {

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            //if (validators.Any())
            //{
            //    var result = await Task.WhenAll(validators.Select(v => v.ValidateAsync(request, cancellationToken)));
            //    var errors = result
            //        .SelectMany(r => r.Errors)
            //        .Where(e => e is not null)
            //        .ToList();
            //    //if (errors.Any())

            //    //var x = Activator.CreateInstance(typeof(TResponse));
            //    return (ResponseDto<TResponse>.Fail(default, string.Join(';', errors), HttpStatusCode.BadRequest) as TResponse);
            //}

            return await next();
        }
    }
}
