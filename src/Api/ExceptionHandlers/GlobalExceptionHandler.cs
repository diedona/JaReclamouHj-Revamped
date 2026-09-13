using JaReclamouHoje.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

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
        if(exception is CustomApplicationException customApplicationException)
        {
            httpContext.Response.StatusCode = (int)customApplicationException.StatusCode;
            var problemDetails = new HttpValidationProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest,
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

        return false;
    }
}
