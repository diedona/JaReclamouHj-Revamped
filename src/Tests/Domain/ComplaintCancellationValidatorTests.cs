using JaReclamouHoje.Domain.Common.Validation;
using JaReclamouHoje.Domain.Entities.Validation;
using JaReclamouHoje.Domain.ValueObjects;

namespace JaReclamouHoje.Tests.Complaints;

public class ComplaintCancellationValidatorTests
{
    private static readonly DateTimeOffset FixedNow = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private static ComplaintCancellationVO Valid(Func<ComplaintCancellationVO>? build = null) =>
        build is null
            ? new ComplaintCancellationVO(Guid.NewGuid(), "Duplicate report.", FixedNow)
            : build();

    private static ComplaintCancellationValidator Validator() => new();

    [Fact]
    public void Valid_Cancellation_Passes()
    {
        var result = Validator().Validate(Valid());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empty_UserId_Fails()
    {
        var cancellation = new ComplaintCancellationVO(Guid.Empty, "Reason.", FixedNow);

        var result = Validator().Validate(cancellation);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ComplaintCancellationVO.CanceledByUserId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Missing_Reason_Fails(string? reason)
    {
        var cancellation = new ComplaintCancellationVO(Guid.NewGuid(), reason!, FixedNow);

        var result = Validator().Validate(cancellation);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ComplaintCancellationVO.Reason));
    }

    [Fact]
    public void Reason_TooLong_Fails()
    {
        var cancellation = new ComplaintCancellationVO(
            Guid.NewGuid(), new string('x', CancellationRules.MaxReasonLength + 1), FixedNow);

        var result = Validator().Validate(cancellation);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ComplaintCancellationVO.Reason));
    }

    [Fact]
    public void Reason_ExactlyMaxLength_Passes()
    {
        var cancellation = new ComplaintCancellationVO(
            Guid.NewGuid(), new string('x', CancellationRules.MaxReasonLength), FixedNow);

        var result = Validator().Validate(cancellation);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Any_Timestamp_Passes()
    {
        // Timestamps are stamped by the service and never validated:
        // past and far-future instants are all accepted here.
        foreach (var instant in new[]
        {
            FixedNow - TimeSpan.FromDays(1),
            FixedNow + TimeSpan.FromHours(1),
        })
        {
            var result = Validator().Validate(
                new ComplaintCancellationVO(Guid.NewGuid(), "Reason.", instant));

            Assert.True(result.IsValid);
        }
    }
}
