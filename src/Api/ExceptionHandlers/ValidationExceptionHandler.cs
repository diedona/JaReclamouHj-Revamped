using FluentValidation;
using JaReclamouHoje.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace JaReclamouHoje.Api.ExceptionHandlers;

public class ValidationExceptionHandler(
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
        if (exception is not ValidationException validationException)
        {
            return false; // Let other exceptions pass through
        }

        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                failureGroup => failureGroup.Key,
                failureGroup => failureGroup.Select(f => f.ErrorMessage).ToArray()
            );

        var problemDetails = new HttpValidationProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation erros ocurred.",
            Errors = errors
        };

        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }
}
