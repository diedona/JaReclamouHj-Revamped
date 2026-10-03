using JaReclamouHoje.Application.Common.Interfaces;
using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Repositories;
using JaReclamouHoje.Domain.Services;
using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.Cancel;

public class CancelComplaintHandler(
    IComplaintRepository complaintRepository,
    ICurrentUser currentUser,
    ComplaintCancellationService cancellationService
) : IRequestHandler<CancelComplaintCommand, ComplaintResponse?>
{
    private readonly IComplaintRepository _complaintRepository = complaintRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ComplaintCancellationService _cancellationService = cancellationService;

    public async Task<ComplaintResponse?> Handle(CancelComplaintCommand request, CancellationToken cancellationToken)
    {
        var complaint = await _complaintRepository.GetByIdAsync(request.Id, cancellationToken);
        if (complaint is null)
        {
            return null;
        }

        var actorIsAdmin = _currentUser.IsInRole(UserRoles.Admin);

        _cancellationService.Cancel(
            complaint,
            _currentUser.UserId!.Value,
            actorIsAdmin,
            request.Reason);

        await _complaintRepository.UpdateAsync(complaint, cancellationToken);

        return ComplaintResponse.FromEntity(complaint);
    }
}
