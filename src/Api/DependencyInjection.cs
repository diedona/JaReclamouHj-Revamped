using Serilog;

namespace JaReclamouHoje.Api;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();

            if (!builder.Configuration.GetSection("Serilog").GetChildren().Any())
                configuration.WriteTo.Console();
        });

        return builder;
    }
}