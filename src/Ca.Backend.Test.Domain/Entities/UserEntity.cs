namespace Ca.Backend.Test.Domain.Entities;

public class UserEntity : BaseEntity
{
    public string? Name { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpirationRefreshToken { get; set; }
    public IList<RoleEntity>? Roles { get; set; }
}
