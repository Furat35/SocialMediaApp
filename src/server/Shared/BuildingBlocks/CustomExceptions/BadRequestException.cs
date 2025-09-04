using BuildingBlocks.Models.Constants;
using System.Net;

namespace BuildingBlocks.CustomExceptions
{
    public class BadRequestException(string message = ErrorMessages.BadRequest, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : BaseException(message, statusCode)
    {
    }
}
