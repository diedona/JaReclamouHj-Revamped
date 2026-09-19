using JaReclamouHoje.Application.Common.Interfaces;
using JaReclamouHoje.Domain.Repositories;
using JaReclamouHoje.Infra.Authentication;
using JaReclamouHoje.Infra.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace JaReclamouHoje.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IComplaintRepository, InMemoryComplaintRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}