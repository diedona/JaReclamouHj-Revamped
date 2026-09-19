namespace JaReclamouHoje.Domain.Entities;

public sealed class User
{
    public Guid Id { get; }
    public string Name { get; }
    public string Email { get; }
    public string PasswordHash { get; }
    public string Role { get; }
    public DateTime CreatedAt { get; }

    private User()
    {
        Name = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
        Role = string.Empty;
    }

    public User(Guid id, string name, string email, string passwordHash, string role, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = createdAt;
    }

    public static User Create(string name, string email, string passwordHash, string role = "User") =>
        new(Guid.NewGuid(), name, email, passwordHash, role, DateTime.UtcNow);
}