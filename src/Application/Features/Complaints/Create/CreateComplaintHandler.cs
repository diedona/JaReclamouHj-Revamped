using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JaReclamouHoje.Application.Features.Complaints.Create;

public class CreateComplaintHandler(
    IComplaintRepository complaintRepository,
    ILogger<CreateComplaintHandler> logger
) : IRequestHandler<CreateComplaintCommand, ComplaintResponse>
{
    private readonly ILogger<CreateComplaintHandler> _logger = logger;
    private readonly IComplaintRepository _complaintRepository = complaintRepository;

    public async Task<ComplaintResponse> Handle(CreateComplaintCommand request, CancellationToken cancellationToken)
    {
        var complaint = Complaint.Create(request.Title, request.Description);
        await _complaintRepository.AddAsync(complaint, cancellationToken);
        return ComplaintResponse.FromEntity(complaint);
    }
}
