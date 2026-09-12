using JaReclamouHoje.Domain.Repositories;
using JaReclamouHoje.Infra.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace JaReclamouHoje.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IComplaintRepository, InMemoryComplaintRepository>();
        return services;
    }
}