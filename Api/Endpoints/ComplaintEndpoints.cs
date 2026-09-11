using JaReclamouHoje.Application.Features.Complaints.Create;
using JaReclamouHoje.Application.Features.Complaints.GetAll;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace JaReclamouHoje.Api.Endpoints;

public static class ComplaintEndpoints
{
    public static IEndpointRouteBuilder MapComplaintEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var group = endpointBuilder.MapGroup("/api/complaints")
            .WithDisplayName("Complaints");

        group.MapGet("/", async (
            ISender mediatr,
            CancellationToken cancellationToken
        ) =>
        {
            var complaints = await mediatr.Send(new GetAllComplaintsQuery(), cancellationToken);
            return TypedResults.Ok(complaints);
        }).WithName("GetComplaints");

        //group.MapGet("/{id}", async Task<Results<Ok<ComplaintResponse>, NotFound>> (
        //    Guid id,
        //    IComplaintService service,
        //    CancellationToken cancellationToken) =>
        //{
        //    var complaint = await service.GetByIdAsync(id, cancellationToken);
        //    return complaint is null 
        //        ? TypedResults.NotFound() 
        //        : TypedResults.Ok(complaint);
        //})
        //    .WithName("GetComplaintById");

        group.MapPost("/", async Task<Created<CreateComplaintResponse>> (
            CreateComplaintCommand request,
            ISender mediatr,
            CancellationToken cancellationToken) =>
        {
            var complaint = await mediatr.Send(request, cancellationToken);
            return TypedResults.Created($"/complaints/{complaint.Id}", complaint);
        })
            .WithName("CreateComplaint");

        return endpointBuilder;
    }
}