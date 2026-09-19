using JaReclamouHoje.Api;
using JaReclamouHoje.Api.Endpoints;
using JaReclamouHoje.Api.ExceptionHandlers;
using JaReclamouHoje.Api.OpenApi;
using JaReclamouHoje.Application;
using JaReclamouHoje.Infra;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
	Log.Information("Starting API...");

	var builder = WebApplication.CreateBuilder(args);

	builder.Services.AddAuthenticationServices(builder.Configuration);
	builder.Services.AddOpenApi(options =>
	{
		options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
	});
	builder.AddSerilog();
	builder.Services.AddApplication();
	builder.Services.AddInfrastructure();
	builder.Services.AddProblemDetails();

	builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
	builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

	var app = builder.Build();

	app.MapOpenApi();

	app.UseExceptionHandler();

	app.UseStatusCodePages();
	app.UseSerilogRequestLogging();
	app.UseHttpsRedirection();

	app.UseAuthentication();
	app.UseAuthorization();

	app.MapAuthEndpoints();
	app.MapComplaintEndpoints();

	app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}