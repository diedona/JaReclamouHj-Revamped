using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.GetAll;

public record GetAllComplaintsQuery() : IRequest<GetAllComplaintsResponse>
{
}
