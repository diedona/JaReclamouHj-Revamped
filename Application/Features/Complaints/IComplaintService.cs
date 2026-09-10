using JaReclamouHoje.Application.Features.Complaints.Dtos;

namespace JaReclamouHoje.Application.Features.Complaints;

public interface IComplaintService
{
    Task<ComplaintResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ComplaintResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ComplaintResponse> CreateAsync(CreateComplaintRequest request, CancellationToken cancellationToken = default);
}