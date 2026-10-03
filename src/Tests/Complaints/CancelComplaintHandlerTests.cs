using FluentValidation;
using JaReclamouHoje.Application.Features.Complaints.Cancel;
using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Exceptions;
using JaReclamouHoje.Domain.Services;
using JaReclamouHoje.Infra.Repositories;
using JaReclamouHoje.Tests;

namespace JaReclamouHoje.Tests.Complaints;

public class CancelComplaintHandlerTests
{
    private static async Task<(InMemoryComplaintRepository Repo, Complaint Complaint, Guid OwnerId)> SeedAsync()
    {
        var repo = new InMemoryComplaintRepository();
        var ownerId = Guid.NewGuid();
        var complaint = Complaint.Create("Late delivery", "Package arrived two weeks late.", ownerId);
        await repo.AddAsync(complaint);
        return (repo, complaint, ownerId);
    }

    private static CancelComplaintHandler Handler(
        InMemoryComplaintRepository repo,
        FakeCurrentUser currentUser) =>
        new(repo, currentUser, new ComplaintCancellationService(TimeProvider.System));

    [Fact]
    public async Task Owner_Cancel_Succeeds()
    {
        var (repo, complaint, ownerId) = await SeedAsync();
        var handler = Handler(repo, new FakeCurrentUser(isAuthenticated: true, userId: ownerId, role: UserRoles.User));

        var response = await handler.Handle(new CancelComplaintCommand(complaint.Id, "No longer needed."), default);

        Assert.NotNull(response);
        Assert.True(response.IsCanceled);
        Assert.True(complaint.IsCanceled);
    }

    [Fact]
    public async Task Admin_Cancel_ForeignComplaint_Succeeds()
    {
        var (repo, complaint, _) = await SeedAsync();
        var handler = Handler(repo, new FakeCurrentUser(isAuthenticated: true, userId: Guid.NewGuid(), role: UserRoles.Admin));

        var response = await handler.Handle(new CancelComplaintCommand(complaint.Id, "Spam."), default);

        Assert.NotNull(response);
        Assert.True(response.IsCanceled);
    }

    [Fact]
    public async Task Unknown_Id_ReturnsNull()
    {
        var (repo, _, ownerId) = await SeedAsync();
        var handler = Handler(repo, new FakeCurrentUser(isAuthenticated: true, userId: ownerId, role: UserRoles.User));

        var response = await handler.Handle(new CancelComplaintCommand(Guid.NewGuid(), "Reason."), default);

        Assert.Null(response);
    }

    [Fact]
    public async Task Stranger_Cancel_ThrowsDenied()
    {
        var (repo, complaint, _) = await SeedAsync();
        var handler = Handler(repo, new FakeCurrentUser(isAuthenticated: true, userId: Guid.NewGuid(), role: UserRoles.User));

        await Assert.ThrowsAsync<ComplaintCancellationDeniedException>(
            () => handler.Handle(new CancelComplaintCommand(complaint.Id, "Trying my luck."), default));

        Assert.False(complaint.IsCanceled);
    }

    [Fact]
    public async Task Empty_Reason_ThrowsValidation_Backstop()
    {
        // The MediatR pipeline validator normally rejects this with a 400 first;
        // the Domain backstop still guards the handler when called directly.
        var (repo, complaint, ownerId) = await SeedAsync();
        var handler = Handler(repo, new FakeCurrentUser(isAuthenticated: true, userId: ownerId, role: UserRoles.User));

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new CancelComplaintCommand(complaint.Id, "   "), default));
    }
}
