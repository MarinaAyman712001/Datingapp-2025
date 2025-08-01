namespace API.entities;

public class AppUser
{
    public string ID { get; set; } = Guid.NewGuid().ToString();
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public required byte[] passwardHash { get; set; }
    public required byte[] passwardSalt { get; set; }
}
