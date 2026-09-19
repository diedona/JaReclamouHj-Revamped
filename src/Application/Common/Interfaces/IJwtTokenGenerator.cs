using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(User user);

    #region [ NESTED CLASSES ]

    public sealed record JwtTokenResult(string Token, int ExpiresInSeconds); 

    #endregion
}