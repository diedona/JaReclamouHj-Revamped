using JaReclamouHoje.Application.Features.Complaints.Create;

namespace JaReclamouHoje.Tests.Complaints;

public class CreateComplaintValidatorTests
{
    private readonly CreateComplaintValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.Validate(new CreateComplaintCommand("Late delivery", "Package arrived late."));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Missing_Description_Fails(string? description)
    {
        var result = _validator.Validate(new CreateComplaintCommand("Late delivery", description!));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateComplaintCommand.Description));
    }

    [Fact]
    public void Short_Title_Fails()
    {
        var result = _validator.Validate(new CreateComplaintCommand("A", "Package arrived late."));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateComplaintCommand.Title));
    }
}
