using JaReclamouHoje.Application.Features.Complaints;
using JaReclamouHoje.Application.Features.Complaints.Dtos;

namespace JaReclamouHoje.Api.Endpoints;

public static class ComplaintEndpoints
{
    public static IEndpointRouteBuilder MapComplaintEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/complaints", async (IComplaintService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetComplaints");

        endpoints.MapGet("/complaints/{id}", async (
            Guid id,
            IComplaintService service,
            CancellationToken cancellationToken) =>
        {
            var complaint = await service.GetByIdAsync(id, cancellationToken);
            return complaint is null ? Results.NotFound() : Results.Ok(complaint);
        })
            .WithName("GetComplaintById");

        endpoints.MapPost("/complaints", async (
            CreateComplaintRequest request,
            IComplaintService service,
            CancellationToken cancellationToken) =>
        {
            var complaint = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/complaints/{complaint.Id}", complaint);
        })
            .WithName("CreateComplaint");

        return endpoints;
    }
}