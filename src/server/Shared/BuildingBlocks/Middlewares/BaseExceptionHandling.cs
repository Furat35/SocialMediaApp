using BuildingBlocks.CustomExceptions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace BuildingBlocks.Middlewares
{
    public static class BaseExceptionHandling
    {
        public static void HandleException(this WebApplication app)
        {
            app.UseExceptionHandler(
               options =>
               {
                   options.Run(async context =>
                   {
                       context.Response.ContentType = "application/json";
                       var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();

                       if (exceptionObject != null)
                       {
                           context.Response.StatusCode = exceptionObject.Error switch
                           {
                               BaseException ex => (int)ex.StatusCode,
                               HttpRequestException ex => StatusCodes.Status400BadRequest,
                               _ => StatusCodes.Status500InternalServerError
                           };
                           var errorMessage = $"{exceptionObject.Error.Message}";
                           if (string.IsNullOrEmpty(errorMessage))
                               errorMessage = ErrorMessages.InternalServerError;

                           var options = new JsonSerializerOptions
                           {
                               PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                               WriteIndented = false
                           };

                           await context.Response
                               .WriteAsync(JsonSerializer.Serialize(
                                   ResponseDto<bool>.Fail(errorMessage, (HttpStatusCode)context.Response.StatusCode), options))
                               .ConfigureAwait(false);
                       }
                   });
               });
        }
    }
}
