using JaReclamouHoje.Application.Features.Complaints.Cancel;
using JaReclamouHoje.Domain.Common.Validation;

namespace JaReclamouHoje.Tests.Complaints;

public class CancelComplaintValidatorTests
{
    private readonly CancelComplaintValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.Validate(new CancelComplaintCommand(Guid.NewGuid(), "Duplicate report."));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empty_Id_Fails()
    {
        var result = _validator.Validate(new CancelComplaintCommand(Guid.Empty, "Reason."));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CancelComplaintCommand.Id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Missing_Reason_Fails(string? reason)
    {
        var result = _validator.Validate(new CancelComplaintCommand(Guid.NewGuid(), reason!));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CancelComplaintCommand.Reason));
    }

    [Fact]
    public void Reason_TooLong_Fails()
    {
        var result = _validator.Validate(
            new CancelComplaintCommand(Guid.NewGuid(), new string('x', CancellationRules.MaxReasonLength + 1)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CancelComplaintCommand.Reason));
    }
}
