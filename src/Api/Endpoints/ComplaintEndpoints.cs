using JaReclamouHoje.Application.Features.Complaints;
using JaReclamouHoje.Application.Features.Complaints.Cancel;
using JaReclamouHoje.Application.Features.Complaints.Create;
using JaReclamouHoje.Application.Features.Complaints.GetAll;
using JaReclamouHoje.Application.Features.Complaints.GetById;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace JaReclamouHoje.Api.Endpoints;

public record CancelComplaintRequest(string Reason);

public static class ComplaintEndpoints
{
    public static RouteGroupBuilder MapComplaintEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/complaints")
            .WithTags("Complaints")
            .WithDisplayName("Complaints")
            .RequireAuthorization();

        group.MapGet("", async (
            ISender sender,
            CancellationToken cancellationToken
        ) =>
        {
            var complaints = await sender.Send(new GetAllComplaintsQuery(), cancellationToken);
            return TypedResults.Ok(complaints);
        }).WithName("GetComplaints");

        group.MapGet("/{id}", async Task<Results<Ok<ComplaintResponse>, NotFound>> (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var complaint = await sender.Send(new GetComplaintByIdQuery(id), cancellationToken);
            return complaint is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(complaint);
        }).WithName("GetComplaintById");

        group.MapPost("", async Task<CreatedAtRoute<ComplaintResponse>> (
            CreateComplaintCommand request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var complaint = await sender.Send(request, cancellationToken);
            return TypedResults.CreatedAtRoute(complaint, "GetComplaintById", new { id = complaint.Id });
        }).WithName("CreateComplaint");

        group.MapPost("/{id}/cancel", async Task<Results<Ok<ComplaintResponse>, NotFound>> (
            Guid id,
            CancelComplaintRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var complaint = await sender.Send(new CancelComplaintCommand(id, request.Reason), cancellationToken);
            return complaint is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(complaint);
        }).WithName("CancelComplaint");

        return group;
    }
}