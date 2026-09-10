using JaReclamouHoje.Application.Features.Complaints;
using Microsoft.Extensions.DependencyInjection;

namespace JaReclamouHoje.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IComplaintService, ComplaintService>();
        return services;
    }
}