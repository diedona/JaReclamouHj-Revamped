using JaReclamouHoje.Application.Common.Interfaces;
using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Repositories;
using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.Create;

public class CreateComplaintHandler(
    IComplaintRepository complaintRepository,
    ICurrentUser currentUser,
    TimeProvider timeProvider
) : IRequestHandler<CreateComplaintCommand, ComplaintResponse>
{
    private readonly IComplaintRepository _complaintRepository = complaintRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<ComplaintResponse> Handle(CreateComplaintCommand request, CancellationToken cancellationToken)
    {
        var complaint = Complaint.Create(
            request.Title, 
            request.Description, 
            _currentUser.UserId!.Value, 
            _timeProvider
        );

        await _complaintRepository.AddAsync(complaint, cancellationToken);
        return ComplaintResponse.FromEntity(complaint);
    }
}
