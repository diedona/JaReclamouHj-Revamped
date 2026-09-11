using JaReclamouHoje.Domain.Abstractions;
using JaReclamouHoje.Domain.Entities;
using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.Create;

public class CreateComplaintHandler(
    IComplaintRepository complaintRepository
) : IRequestHandler<CreateComplaintCommand, CreateComplaintResponse>
{
    private readonly IComplaintRepository _complaintRepository = complaintRepository;

    public async Task<CreateComplaintResponse> Handle(CreateComplaintCommand request, CancellationToken cancellationToken)
    {
        var complaint = Complaint.Create(request.Title, request.Description);
        await _complaintRepository.AddAsync(complaint, cancellationToken);
        return CreateComplaintResponse.FromEntity(complaint);
    }
}
