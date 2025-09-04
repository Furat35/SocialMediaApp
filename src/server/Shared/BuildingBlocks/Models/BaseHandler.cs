using BuildingBlocks.Data;
using BuildingBlocks.Helpers;
using BuildingBlocks.Models.Constants;
using System.Net;

namespace BuildingBlocks.Models
{
    public class BaseHandler<TRepository, TEntity>(TRepository repository)
        where TEntity : class, new()
        where TRepository : class, IGenericRepository<TEntity>
    {
        protected readonly TRepository _repository = repository;

        protected ResponseDto<TResponse> ReturnSuccess<TResponse>(TResponse data = default, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return ResponseDto<TResponse>.Success(data, statusCode);
        }

        protected ResponseDto<TResponse> ReturnFail<TResponse>(TResponse data, string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return ResponseDto<TResponse>.Fail(data, message, statusCode);
        }

        protected ResponseDto<TResponse> ReturnFail<TResponse>(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return ResponseDto<TResponse>.Fail(default, message, statusCode);
        }

        protected ResponseDto<T> ToResponse<T>(HttpStatusCode resp)
        {
            return resp.ToResponse<T>();
        }

        protected async Task<ResponseDto<bool>> SaveChangesAsync()
        {
            var saveResult = await repository.SaveChangesAsync();
            return saveResult > 0
                ? ResponseDto<bool>.Success(true, HttpStatusCode.OK)
                : ResponseDto<bool>.Fail(ErrorMessages.SaveChangesError, HttpStatusCode.InternalServerError);
        }
    }
}
