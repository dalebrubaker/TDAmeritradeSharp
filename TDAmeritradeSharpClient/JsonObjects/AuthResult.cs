namespace TDAmeritradeSharpClient;

[Serializable]
public class AuthResult
{
    public string RedirectUrl { get; set; } = "https://127.0.0.1";
    public string ConsumerKey { get; set; } = "";
    public string SecurityCode { get; set; } = "";
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public string Scope { get; set; } = "";
    public int ExpiresIn { get; set; }
    public int RefreshTokenExpiresIn { get; set; }
    public string TokenType { get; set; } = "";
}