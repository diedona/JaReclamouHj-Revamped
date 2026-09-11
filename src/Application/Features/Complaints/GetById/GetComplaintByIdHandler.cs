using JaReclamouHoje.Domain.Abstractions;
using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.GetById;

public class GetComplaintByIdHandler(
    IComplaintRepository complaintRepository
) : IRequestHandler<GetComplaintByIdQuery, ComplaintResponse?>
{
    private readonly IComplaintRepository _complaintRepository = complaintRepository;

    public async Task<ComplaintResponse?> Handle(GetComplaintByIdQuery request, CancellationToken cancellationToken)
    {
        var complaint = await _complaintRepository.GetByIdAsync(request.Id, cancellationToken);
        return complaint is null 
            ? null 
            : ComplaintResponse.FromEntity(complaint);
    }
}