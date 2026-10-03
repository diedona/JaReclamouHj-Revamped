namespace JaReclamouHoje.Domain.Common.Validation;

/// <summary>
/// Single source of truth for complaint-cancellation input limits.
/// Pure data (no validation logic): referenced by Domain backstop validators
/// and by Application command validators. Entities must not duplicate these.
/// </summary>
public static class CancellationRules
{
    /// <summary>Maximum length of the cancellation reason.</summary>
    public const int MaxReasonLength = 500;
}
