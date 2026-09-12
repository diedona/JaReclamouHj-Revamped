using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Repositories;
using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.Create;

public class CreateComplaintHandler(
    IComplaintRepository complaintRepository
) : IRequestHandler<CreateComplaintCommand, ComplaintResponse>
{
    private readonly IComplaintRepository _complaintRepository = complaintRepository;

    public async Task<ComplaintResponse> Handle(CreateComplaintCommand request, CancellationToken cancellationToken)
    {
        var complaint = Complaint.Create(request.Title, request.Description);
        await _complaintRepository.AddAsync(complaint, cancellationToken);
        return ComplaintResponse.FromEntity(complaint);
    }
}
