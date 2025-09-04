using BuildingBlocks.Models.Constants;
using System.Net;

namespace BuildingBlocks.CustomExceptions
{
    public class NotFoundException(string message = ErrorMessages.NotFound, HttpStatusCode statusCode = HttpStatusCode.NotFound) : BaseException(message, statusCode)
    {
    }
}
