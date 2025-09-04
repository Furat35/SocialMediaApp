using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using System.Net;

namespace BuildingBlocks.Helpers
{
    public static class ResultMapper
    {
        public static ResponseDto<T> ToResponse<T>(this HttpStatusCode result, T value = default, string errorMessage = null) =>
            result switch
            {
                HttpStatusCode.OK => ResponseDto<T>.Success(value, HttpStatusCode.OK),
                HttpStatusCode.NotFound => ResponseDto<T>.Fail(errorMessage ?? ErrorMessages.NotFound, HttpStatusCode.NotFound),
                HttpStatusCode.Forbidden => ResponseDto<T>.Fail(errorMessage ?? ErrorMessages.Forbidden, HttpStatusCode.Forbidden),
                _ => ResponseDto<T>.Fail(errorMessage ?? "Unknown error.", HttpStatusCode.InternalServerError)
            };
    }
}
