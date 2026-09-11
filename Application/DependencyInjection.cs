using Microsoft.Extensions.DependencyInjection;

namespace JaReclamouHoje.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }

    /// <summary>
    /// Custom Middleware to add Mediatr into the project
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMediatr(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly)
        );

        return services;
    }
}