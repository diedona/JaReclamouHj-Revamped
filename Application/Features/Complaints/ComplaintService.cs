using JaReclamouHoje.Application.Features.Complaints.Dtos;
using JaReclamouHoje.Domain.Abstractions;
using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Application.Features.Complaints;

public sealed class ComplaintService : IComplaintService
{
    private readonly IComplaintRepository _repository;

    public ComplaintService(IComplaintRepository repository)
    {
        _repository = repository;
    }

    public async Task<ComplaintResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var complaint = await _repository.GetByIdAsync(id, cancellationToken);
        return complaint is null ? null : ComplaintResponse.FromEntity(complaint);
    }

    public async Task<IReadOnlyList<ComplaintResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var complaints = await _repository.GetAllAsync(cancellationToken);
        return complaints.Select(ComplaintResponse.FromEntity).ToList();
    }

    public async Task<ComplaintResponse> CreateAsync(CreateComplaintRequest request, CancellationToken cancellationToken = default)
    {
        var complaint = Complaint.Create(request.Title, request.Description);
        await _repository.AddAsync(complaint, cancellationToken);
        return ComplaintResponse.FromEntity(complaint);
    }
}