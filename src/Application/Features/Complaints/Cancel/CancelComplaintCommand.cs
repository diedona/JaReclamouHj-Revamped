using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.Cancel;

/// <summary>
/// Server timestamps the cancellation, so the command carries no timestamp:
/// the future-date input problem disappears at the boundary and the Domain
/// backstop validator only guards direct Domain construction.
/// </summary>
public record CancelComplaintCommand(
    Guid Id,
    string Reason
) : IRequest<ComplaintResponse?>;
