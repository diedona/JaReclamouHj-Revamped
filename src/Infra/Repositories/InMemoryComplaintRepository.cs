using System.Collections.Concurrent;
using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Repositories;

namespace JaReclamouHoje.Infra.Repositories;

public sealed class InMemoryComplaintRepository : IComplaintRepository
{
    private readonly ConcurrentDictionary<Guid, Complaint> _complaints = new();

    public Task<Complaint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _complaints.TryGetValue(id, out var complaint);
        return Task.FromResult(complaint);
    }

    public Task<IReadOnlyList<Complaint>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Complaint>>(_complaints.Values.ToList());

    public Task AddAsync(Complaint complaint, CancellationToken cancellationToken = default)
    {
        _complaints[complaint.Id] = complaint;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Complaint complaint, CancellationToken cancellationToken = default)
    {
        _complaints[complaint.Id] = complaint;
        return Task.CompletedTask;
    }
}