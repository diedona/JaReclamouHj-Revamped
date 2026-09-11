using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Application.Features.Complaints.Create;

public record CreateComplaintResponse(
    Guid Id,
    string Title,
    string Description,
    ComplaintStatus Status,
    DateTime CreatedAt
)
{
    public static CreateComplaintResponse FromEntity(Complaint complaint) =>
        new(complaint.Id, complaint.Title, complaint.Description, complaint.Status, complaint.CreatedAt);
}
