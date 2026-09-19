using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Application.Features.Auth;

public record AuthResponse(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Token,
    int ExpiresInSeconds)
{
    public static AuthResponse Create(User user, string token, int expiresInSeconds) =>
        new(user.Id, user.Name, user.Email, user.Role, token, expiresInSeconds);
}