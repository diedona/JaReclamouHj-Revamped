using JaReclamouHoje.Domain.Abstractions;
using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.GetAll;

public class GetAllComplaintsHandler(
    IComplaintRepository complaintRepository
) : IRequestHandler<GetAllComplaintsQuery, GetAllComplaintsResponse>
{
    private readonly IComplaintRepository _complaintRepository = complaintRepository;

    public async Task<GetAllComplaintsResponse> Handle(GetAllComplaintsQuery request, CancellationToken cancellationToken)
    {
        var complaints = await _complaintRepository.GetAllAsync(cancellationToken);
        return GetAllComplaintsResponse.CreateFromEntities(complaints.AsEnumerable());
    }
}
