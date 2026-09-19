using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}