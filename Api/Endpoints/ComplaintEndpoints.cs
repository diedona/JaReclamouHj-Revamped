using JaReclamouHoje.Application.Features.Complaints;
using JaReclamouHoje.Application.Features.Complaints.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace JaReclamouHoje.Api.Endpoints;

public static class ComplaintEndpoints
{
    public static IEndpointRouteBuilder MapComplaintEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/complaints", async (IComplaintService service, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetComplaints");

        endpoints.MapGet("/complaints/{id}", async Task<Results<Ok<ComplaintResponse>, NotFound>> (
            Guid id,
            IComplaintService service,
            CancellationToken cancellationToken) =>
        {
            var complaint = await service.GetByIdAsync(id, cancellationToken);
            return complaint is null 
                ? TypedResults.NotFound() 
                : TypedResults.Ok(complaint);
        })
            .WithName("GetComplaintById");

        endpoints.MapPost("/complaints", async Task<Created<ComplaintResponse>> (
            CreateComplaintRequest request,
            IComplaintService service,
            CancellationToken cancellationToken) =>
        {
            var complaint = await service.CreateAsync(request, cancellationToken);
            return TypedResults.Created($"/complaints/{complaint.Id}", complaint);
        })
            .WithName("CreateComplaint");

        return endpoints;
    }
}