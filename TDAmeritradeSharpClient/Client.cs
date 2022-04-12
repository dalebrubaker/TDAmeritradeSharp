using System.Diagnostics;
using System.Net;
using System.Security.Authentication;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TDAmeritradeSharpClient;

public class Client : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Client> _logger;
    private List<DateTime> _requestTimesUtc = new List<DateTime>();
    public const string Success = "Authorization was successful";

    /// <summary>
    /// The fully-qualified path to the json file in Users that holds the Authorization information 
    /// </summary>
    public string PathAuthResult { get; }

    public Client(ILogger<Client> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        var userSettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(TDAmeritradeSharpClient));
        if (!Directory.Exists(userSettingsDirectory))
        {
            Directory.CreateDirectory(userSettingsDirectory);
        }
        PathAuthResult = Path.Combine(userSettingsDirectory, $"{nameof(TDAmeritradeSharpClient)}.json");
        LoadAuthResult();
    }

    public TDAuthResult AuthResult { get; private set; } = new TDAuthResult();

    /// <summary>
    /// A list of the DateTime.UtcNow when each request was made, pruned to the last minute
    /// </summary>
    public List<DateTime> RequestTimesUtc
    {
        get
        {
            if (_requestTimesUtc.Count > 0)
            {}
            var firstAllowed = DateTime.UtcNow.AddMinutes(-1);
            for (var i = 0; i < _requestTimesUtc.Count; i++)
            {
                var timestamp = _requestTimesUtc[i];
                if (timestamp < firstAllowed)
                {
                    _requestTimesUtc.RemoveAt(i--);
                }
            }
            return _requestTimesUtc;
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    /// <summary>
    ///     Set the AuthResult file, returning an error message if there is an error
    /// </summary>
    /// <param name="code"></param>
    /// <param name="consumerKey"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public async Task<string> SetAuthResults(string code, string consumerKey, string callback)
    {
        var decoded = HttpUtility.UrlDecode(code);
        var path = "https://api.tdameritrade.com/v1/oauth2/token";
        var dict = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "access_type", "offline" },
            { "client_id", $"{consumerKey}@AMER.OAUTHAP" },
            { "redirect_uri", callback },
            { "code", decoded }
        };
        var req = new HttpRequestMessage(HttpMethod.Post, path) { Content = new FormUrlEncodedContent(dict) };
        var res = await SendRequest(req);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                var json = await res.Content.ReadAsStringAsync();
                var authResult = JsonConvert.DeserializeObject<TDAuthResult>(json) ?? new TDAuthResult();
                authResult.security_code = code;
                authResult.consumer_key = consumerKey;
                authResult.redirect_url = callback;
                authResult.CreationTimestampUtc = DateTime.UtcNow;
                SaveAuthResult(authResult);
                LoadAuthResult();
                break;
            default:
                return $"Bad request: {res.StatusCode} {res.ReasonPhrase}";
        }
        return Success;
    }

    /// <summary>
    /// Use this method to send ALL requests to HttpClient
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage req)
    {
        RequestTimesUtc.Add(DateTime.UtcNow);
        var res = await _httpClient.SendAsync(req);
        return res;
    }

    private void LoadAuthResult()
    {
        if (!File.Exists(PathAuthResult))
        {
            return;
        }
        var json = File.ReadAllText(PathAuthResult);
        AuthResult = JsonConvert.DeserializeObject<TDAuthResult>(json);
    }

    private void SaveAuthResult(TDAuthResult authResult)
    {
        var jsonSave = JsonConvert.SerializeObject(authResult);
        File.WriteAllText(PathAuthResult, jsonSave);
    }

    /// <summary>
    /// Get a new access token before it expires
    /// Get a new refresh token before it expires
    /// </summary>
    public async Task RequireNotExpiredTokensAsync()
    {
        if (AuthResult.RefreshTokenExpirationUtc - DateTime.UtcNow < TimeSpan.FromDays(7))
        {
            // Need a new refresh token
            await GetNewRefreshTokenAsync().ConfigureAwait(false);
        }
        if (AuthResult.AccessTokenExpirationUtc - DateTime.UtcNow < TimeSpan.FromMinutes(1))
        {
            // Need a new access token
            await GetNewAccessTokenAsync().ConfigureAwait(false);
        }
        
    }

    private async Task GetNewRefreshTokenAsync()
    {
        if (AuthResult.refresh_token == null)
        {
            // Never authorized 
            return;
        }
        var path = "https://api.tdameritrade.com/v1/oauth2/token";
        var dict = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", AuthResult.refresh_token },
            { "access_type", "offline" },
            { "client_id", $"{AuthResult.consumer_key}@AMER.OAUTHAP" }
        };
        var req = new HttpRequestMessage(HttpMethod.Post, path) { Content = new FormUrlEncodedContent(dict) };
        var res = await SendRequest(req);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                var json = await res.Content.ReadAsStringAsync();
                var authResult = JsonConvert.DeserializeObject<TDAuthResult>(json) ?? new TDAuthResult();
                AuthResult.access_token = authResult.access_token;
                AuthResult.refresh_token = authResult.refresh_token;
                AuthResult.expires_in = authResult.expires_in;
                AuthResult.refresh_token_expires_in = authResult.refresh_token_expires_in;
                AuthResult.CreationTimestampUtc = DateTime.UtcNow;
                SaveAuthResult(authResult);
                LoadAuthResult();
                break;
            default:
                throw new AuthenticationException($"Not able to get a new Access Token. {res.StatusCode} {res.ReasonPhrase}");
        }
    }
    
    private async Task GetNewAccessTokenAsync()
    {
        if (AuthResult.refresh_token == null)
        {
            // Never authorized 
            return;
        }
        var path = "https://api.tdameritrade.com/v1/oauth2/token";
        var dict = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", AuthResult.refresh_token },
            { "client_id", $"{AuthResult.consumer_key}@AMER.OAUTHAP" }
        };
        var req = new HttpRequestMessage(HttpMethod.Post, path) { Content = new FormUrlEncodedContent(dict) };
        var res = await SendRequest(req);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                var json = await res.Content.ReadAsStringAsync();
                var authResult = JsonConvert.DeserializeObject<TDAuthResult>(json) ?? new TDAuthResult();
                AuthResult.access_token = authResult.access_token;
                AuthResult.refresh_token = authResult.refresh_token;
                AuthResult.expires_in = authResult.expires_in;
                AuthResult.refresh_token_expires_in = authResult.refresh_token_expires_in;
                AuthResult.CreationTimestampUtc = DateTime.UtcNow;
                SaveAuthResult(authResult);
                LoadAuthResult();
                break;
            default:
                throw new AuthenticationException($"Not able to get a new Access Token. {res.StatusCode} {res.ReasonPhrase}");
        }
    }
}