namespace Ca.Backend.Test.Application.Models.Request;

public class UserRequest
{
    public string? Name { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public IList<Guid>? RoleIds { get; set; }
}
