using System.Net;

namespace BuildingBlocks.CustomExceptions
{
    public class BaseException(string exceptionMessage, HttpStatusCode statusCode) : Exception(exceptionMessage)
    {
        public HttpStatusCode StatusCode { get; set; } = statusCode;
        public string ExceptionMessage { get; set; } = exceptionMessage;
    }
}
