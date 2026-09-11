using JaReclamouHoje.Api.Endpoints;
using JaReclamouHoje.Application;
using JaReclamouHoje.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddMediatr();
builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapComplaintEndpoints();

app.Run();