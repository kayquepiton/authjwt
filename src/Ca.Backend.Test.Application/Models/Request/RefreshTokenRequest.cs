namespace Ca.Backend.Test.Application.Models.Request;

public class RefreshTokenRequest
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
