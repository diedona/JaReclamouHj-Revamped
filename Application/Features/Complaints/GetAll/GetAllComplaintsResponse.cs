using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Application.Features.Complaints.GetAll;

public record GetAllComplaintsResponse(IEnumerable<ComplaintResponse> Complaints)
{
    public static GetAllComplaintsResponse CreateFromEntities(IEnumerable<Complaint> complaints)
    {
        var complaintsResponse = complaints.Select(ComplaintResponse.FromEntity);

        return new(complaintsResponse);
    }
}