using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Domain.Repositories;

public interface IComplaintRepository
{
    Task<Complaint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Complaint>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Complaint complaint, CancellationToken cancellationToken = default);
}