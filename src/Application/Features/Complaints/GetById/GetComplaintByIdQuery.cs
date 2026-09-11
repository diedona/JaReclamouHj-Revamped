using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.GetById;

public sealed record GetComplaintByIdQuery(Guid Id) : IRequest<ComplaintResponse?>;