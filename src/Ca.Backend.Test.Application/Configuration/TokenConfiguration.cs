namespace Ca.Backend.Test.Application.Configuration;

public class TokenConfiguration
{
    public required string Secret { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpirationInMinutesAccessToken { get; set; }
    public int ExpirationInMinutesRefreshToken { get; set; }
}

