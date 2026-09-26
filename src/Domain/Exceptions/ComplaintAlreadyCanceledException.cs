namespace JaReclamouHoje.Domain.Exceptions;

/// <summary>Thrown when attempting to cancel a complaint that is already canceled.</summary>
public sealed class ComplaintAlreadyCanceledException : InvalidOperationException
{
    public ComplaintAlreadyCanceledException()
        : base("The complaint is already canceled.")
    {
    }

    public ComplaintAlreadyCanceledException(Guid complaintId)
        : base($"The complaint '{complaintId}' is already canceled.")
    {
    }
}
