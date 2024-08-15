namespace Ca.Backend.Test.Domain.Entities;

public class RoleEntity : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IList<UserEntity>? Users { get; set; }
}
