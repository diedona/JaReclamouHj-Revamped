using JaReclamouHoje.Application.Features.Auth.Login;
using JaReclamouHoje.Application.Features.Auth.Register;
using MediatR;

namespace JaReclamouHoje.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth")
            .WithDisplayName("Auth");

        group.MapPost("/register", async (
            RegisterUserCommand request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return TypedResults.Ok(response);
        }).WithName("RegisterUser");

        group.MapPost("/login", async (
            LoginUserCommand request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return TypedResults.Ok(response);
        }).WithName("LoginUser");

        return group;
    }
}