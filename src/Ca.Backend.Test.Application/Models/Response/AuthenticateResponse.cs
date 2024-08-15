namespace Ca.Backend.Test.Application.Models.Response;

public class AuthenticateResponse
{
    public bool Authenticated { get; set; }
    public DateTime Created { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}