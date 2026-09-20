using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JaReclamouHoje.Application.Common.Interfaces;

namespace JaReclamouHoje.Api.Authentication;

public sealed class HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated is true;

    public Guid? UserId =>
        Guid.TryParse(User?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : null;

    public string? Role => User?.FindFirstValue(ClaimTypes.Role);

    public bool IsInRole(string role) => IsAuthenticated && User!.IsInRole(role);
}
