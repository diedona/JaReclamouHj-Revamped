using JaReclamouHoje.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace JaReclamouHoje.Api.ExceptionHandlers;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService
) : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken
    )
    {
        if (exception is not CustomApplicationException customApplicationException)
        {
            return false;
        }

        httpContext.Response.StatusCode = (int)customApplicationException.StatusCode;
        var problemDetails = new HttpValidationProblemDetails()
        {
            Status = customApplicationException.StatusCode switch
            {
                HttpStatusCode.BadRequest => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            },
            Title = "An error ocurred",
            Detail = customApplicationException.Message
        };

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
        
    }
}
