using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Api.Authorization;

public static class PoliciesExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services
            .AddAuthorizationBuilder()
            .AddPolicy(Policies.AdminOnly, policy => policy.RequireRole(UserRoles.Admin));

        return services;
    }
}
