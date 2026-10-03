using FluentValidation;
using JaReclamouHoje.Domain.Common.Validation;
using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Exceptions;
using JaReclamouHoje.Domain.Services;
using JaReclamouHoje.Domain.ValueObjects;

namespace JaReclamouHoje.Tests.Complaints;

public class ComplaintCancellationTests
{
    private static Complaint CreateOwnedComplaint(Guid? ownerId = null) =>
        Complaint.Create("Late delivery", "Package arrived two weeks late.", ownerId ?? Guid.NewGuid());

    [Fact]
    public void Create_SetsOwnership()
    {
        var ownerId = Guid.NewGuid();

        var complaint = CreateOwnedComplaint(ownerId);

        Assert.Equal(ownerId, complaint.CreatedByUserId);
        Assert.False(complaint.IsCanceled);
        Assert.Null(complaint.Cancellation);
    }

    [Fact]
    public void Create_WithoutOwner_ThrowsValidation()
    {
        Assert.Throws<ValidationException>(() => Complaint.Create("Title", "Description", Guid.Empty));
    }

    [Fact]
    public void Create_WithoutTitle_ThrowsValidation()
    {
        Assert.Throws<ValidationException>(() => Complaint.Create("  ", "Description", Guid.NewGuid()));
    }

    [Fact]
    public void Cancel_RecordsWhenWhoAndWhy()
    {
        var complaint = CreateOwnedComplaint();
        var canceledBy = Guid.NewGuid();
        var at = DateTimeOffset.UtcNow;

        complaint.Cancel(ComplaintCancellationVO.Create(canceledBy, "Duplicate report.", at));

        Assert.True(complaint.IsCanceled);
        Assert.NotNull(complaint.Cancellation);
        Assert.Equal(canceledBy, complaint.Cancellation.CanceledByUserId);
        Assert.Equal("Duplicate report.", complaint.Cancellation.Reason);
        Assert.Equal(at, complaint.Cancellation.CanceledAt);
    }

    [Fact]
    public void Cancel_TrimsReason()
    {
        var complaint = CreateOwnedComplaint();

        complaint.Cancel(ComplaintCancellationVO.Create(Guid.NewGuid(), "  Duplicate report.  ", DateTimeOffset.UtcNow));

        Assert.Equal("Duplicate report.", complaint.Cancellation!.Reason);
    }

    [Fact]
    public void Cancel_Twice_Throws()
    {
        var complaint = CreateOwnedComplaint();
        complaint.Cancel(ComplaintCancellationVO.Create(Guid.NewGuid(), "First reason.", DateTimeOffset.UtcNow));

        Assert.Throws<ComplaintAlreadyCanceledException>(
            () => complaint.Cancel(ComplaintCancellationVO.Create(Guid.NewGuid(), "Second reason.", DateTimeOffset.UtcNow)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Cancel_WithoutReason_ThrowsValidation(string? reason)
    {
        var complaint = CreateOwnedComplaint();

        Assert.Throws<ValidationException>(
            () => complaint.Cancel(ComplaintCancellationVO.Create(Guid.NewGuid(), reason!, DateTimeOffset.UtcNow)));
    }

    [Fact]
    public void Cancel_ReasonTooLong_ThrowsValidation()
    {
        var complaint = CreateOwnedComplaint();

        Assert.Throws<ValidationException>(
            () => complaint.Cancel(ComplaintCancellationVO.Create(Guid.NewGuid(), new string('x', CancellationRules.MaxReasonLength + 1), DateTimeOffset.UtcNow)));
    }

    [Fact]
    public void Service_Owner_CanCancel()
    {
        var ownerId = Guid.NewGuid();
        var complaint = CreateOwnedComplaint(ownerId);
        var service = new ComplaintCancellationService(TimeProvider.System);

        Assert.True(service.CanCancel(complaint, ownerId, actorIsAdmin: false));

        service.Cancel(complaint, ownerId, actorIsAdmin: false, "No longer needed.");

        Assert.True(complaint.IsCanceled);
    }

    [Fact]
    public void Service_Admin_CanCancelForeignComplaint()
    {
        var complaint = CreateOwnedComplaint();
        var adminId = Guid.NewGuid();
        var service = new ComplaintCancellationService(TimeProvider.System);

        Assert.True(service.CanCancel(complaint, adminId, actorIsAdmin: true));

        service.Cancel(complaint, adminId, actorIsAdmin: true, "Spam.");

        Assert.True(complaint.IsCanceled);
    }

    [Fact]
    public void Service_Stranger_CannotCancel()
    {
        var complaint = CreateOwnedComplaint();
        var service = new ComplaintCancellationService(TimeProvider.System);

        Assert.False(service.CanCancel(complaint, Guid.NewGuid(), actorIsAdmin: false));

        Assert.Throws<ComplaintCancellationDeniedException>(
            () => service.Cancel(complaint, Guid.NewGuid(), actorIsAdmin: false, "Trying my luck."));

        Assert.False(complaint.IsCanceled);
    }

    [Fact]
    public void Service_AlreadyCanceled_ThrowsAlreadyCanceled()
    {
        var ownerId = Guid.NewGuid();
        var complaint = CreateOwnedComplaint(ownerId);
        var service = new ComplaintCancellationService(TimeProvider.System);
        service.Cancel(complaint, ownerId, actorIsAdmin: false, "Done.");

        // Even an admin cannot cancel twice.
        Assert.Throws<ComplaintAlreadyCanceledException>(
            () => service.Cancel(complaint, Guid.NewGuid(), actorIsAdmin: true, "Again."));
    }
}
