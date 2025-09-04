using BuildingBlocks.Models.Constants;
using System.Net;

namespace BuildingBlocks.CustomExceptions
{
    public class ForbiddenException(string message = ErrorMessages.Forbidden, HttpStatusCode statusCode = HttpStatusCode.Forbidden) : BaseException(message, statusCode)
    {
    }
}
