using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Application.Features.Complaints;

public record ComplaintResponse(
    Guid Id,
    string Title,
    string Description,
    ComplaintStatus Status,
    DateTime CreatedAt
)
{
    public static ComplaintResponse FromEntity(Complaint complaint) =>
        new(complaint.Id, complaint.Title, complaint.Description, complaint.Status, complaint.CreatedAt);
}