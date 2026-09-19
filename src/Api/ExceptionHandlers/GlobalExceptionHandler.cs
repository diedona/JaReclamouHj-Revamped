using JaReclamouHoje.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace JaReclamouHoje.Api.ExceptionHandlers;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        var statusCode = exception switch
        {
            CustomApplicationException customException => customException.StatusCode,
            BadHttpRequestException badHttpRequestException => badHttpRequestException.StatusCode,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = exception switch
            {
                CustomApplicationException => "An application error occurred.",
                BadHttpRequestException => "Bad Request",
                _ => "An unexpected error occurred."
            },
            Detail = exception switch
            {
                CustomApplicationException customException => customException.Message,
                BadHttpRequestException => "Your request seems to be invalid. Check required parameters in URL and/or body.",
                _ => "Please try again later."
            }
        };

        httpContext.Response.StatusCode = statusCode;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }
}