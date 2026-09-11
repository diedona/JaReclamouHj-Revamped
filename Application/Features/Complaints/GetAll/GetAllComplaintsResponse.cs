using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Application.Features.Complaints.GetAll;

public record GetAllComplaintsResponse(IEnumerable<ComplaintResponse> Complaints)
{
    public static GetAllComplaintsResponse CreateFromEntities(IEnumerable<Complaint> complaints)
    {
        var complaintsResponse = complaints.Select(c =>
            new ComplaintResponse(
                c.Id,
                c.Title,
                c.Description,
                c.Status,
                c.CreatedAt
            )
        );

        return new(complaintsResponse);
    }
}

public record ComplaintResponse(Guid Id, string Title, string Description, ComplaintStatus Status, DateTime CreatedAt);