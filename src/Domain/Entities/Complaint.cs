namespace JaReclamouHoje.Domain.Entities;

public sealed class Complaint
{
    public Guid Id { get; }
    public string Title { get; }
    public string Description { get; }
    public ComplaintStatus Status { get; }
    public DateTime CreatedAt { get; }

    private Complaint()
    {
        Title = string.Empty;
        Description = string.Empty;
    }

    public Complaint(Guid id, string title, string description, ComplaintStatus status, DateTime createdAt)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        CreatedAt = createdAt;
    }

    public static Complaint Create(string title, string description) =>
        new(Guid.NewGuid(), title, description, ComplaintStatus.Pending, DateTime.UtcNow);
}