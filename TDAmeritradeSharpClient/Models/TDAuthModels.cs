// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace TDAmeritradeSharpClient;

public class TDAuthValues
{
#pragma warning disable CS8618
    public TDAuthValues()
#pragma warning restore CS8618
    {
        // for JSON deserialization
    }
    
    public TDAuthValues(string redirectUrl, string consumerKey, TDAuthResponse authResponse) : this()
    {
        RedirectUrl = redirectUrl;
        ConsumerKey = consumerKey;
        AccessToken = authResponse.access_token ?? throw new InvalidOperationException();;
        RefreshToken = authResponse.refresh_token ?? throw new InvalidOperationException();
        AccessTokenExpirationUtc = DateTime.UtcNow.AddSeconds(authResponse.expires_in);
        RefreshTokenExpirationUtc = DateTime.UtcNow.AddSeconds(authResponse.refresh_token_expires_in);
    }

    public string RedirectUrl { get; set; }
    public string ConsumerKey { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpirationUtc { get; set; }
    public DateTime RefreshTokenExpirationUtc { get; set; }
}

[Serializable]
public class TDAuthResponse
{
    public string? access_token { get; set; }
    public string? refresh_token { get; set; }
    public string? token_type { get; set; }
    public int expires_in { get; set; }
    public string? scope { get; set; }
    public int refresh_token_expires_in { get; set; }
}