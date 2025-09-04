using System.Net;

namespace BuildingBlocks.Models
{
    public class ResponseDto<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool IsError { get; set; }
        public T Data { get; set; }

        public static ResponseDto<T> Fail(T data, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new() { Data = data, ErrorMessages = [message], StatusCode = statusCode, IsError = true };

        public static ResponseDto<T> Fail(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new() { ErrorMessages = [message], StatusCode = statusCode, IsError = true };

        public static ResponseDto<T> Fail(List<string> messages, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new() { ErrorMessages = messages, StatusCode = statusCode, IsError = true };

        public static ResponseDto<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
            => new() { StatusCode = statusCode, IsError = false, Data = data };

        public static ResponseDto<T> Success(HttpStatusCode statusCode = HttpStatusCode.OK)
            => new() { StatusCode = statusCode, IsError = false, Data = default };
        //public static ResponseDto<T> ConditionalResponse(bool isSuccess)
        //    => new() { IsError = !isSuccess };

        //public static ResponseDto<T> ConditionalResponse(
        //    bool isSuccess = false,
        //    string message = "Error Occured!",
        //    HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        //    T data = default)
        //    => new()
        //    {
        //        IsError = !isSuccess,
        //        ErrorMessages = !isSuccess ? [message] : [],
        //        StatusCode = statusCode,
        //        Data = data
        //    };
    }

}
