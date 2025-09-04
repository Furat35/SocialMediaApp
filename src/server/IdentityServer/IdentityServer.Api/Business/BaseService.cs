using BuildingBlocks.Data;
using BuildingBlocks.Helpers;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using System.Net;

namespace IdentityServer.Api.Business
{
    public class BaseService<TRepository, TEntity>(TRepository repository)
        where TEntity : BaseEntity
        where TRepository : IGenericRepository<TEntity>
    {
        protected TRepository Repository => repository;

        protected ResponseDto<TResponse> ReturnSuccess<TResponse>(TResponse data = default, HttpStatusCode statusCode = HttpStatusCode.OK)
            => ResponseDto<TResponse>.Success(data, statusCode);

        protected ResponseDto<TResponse> ReturnFail<TResponse>(TResponse data, string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            => ResponseDto<TResponse>.Fail(data, message, statusCode);

        protected ResponseDto<TResponse> ReturnFail<TResponse>(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            => ResponseDto<TResponse>.Fail(default, message, statusCode);

        protected ResponseDto<T> ToResponse<T>(HttpStatusCode resp)
        {
            return resp.ToResponse<T>();
        }

        public async Task<ResponseDto<bool>> SaveChangesAsync()
        {
            return await Repository.SaveChangesAsync() != 0
                ? ResponseDto<bool>.Success()
                : ResponseDto<bool>.Fail(ErrorMessages.SaveChangesError, HttpStatusCode.InternalServerError);
        }
    }
}
