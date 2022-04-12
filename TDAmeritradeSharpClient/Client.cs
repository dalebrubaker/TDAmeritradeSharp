using System.Net;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TDAmeritradeSharpClient;

public class Client : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Client> _logger;
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
        using var client = new HttpClient();
        var dict = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "access_type", "offline" },
            { "client_id", $"{consumerKey}@AMER.OAUTHAP" },
            { "redirect_uri", callback },
            { "code", decoded }
        };
        var req = new HttpRequestMessage(HttpMethod.Post, path) { Content = new FormUrlEncodedContent(dict) };
        var res = await client.SendAsync(req);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                var json = await res.Content.ReadAsStringAsync();
                var authResult = JsonConvert.DeserializeObject<TDAuthResult>(json) ?? new TDAuthResult();
                authResult.security_code = code;
                authResult.consumer_key = consumerKey;
                authResult.redirect_url = callback;
                SaveAuthResult(authResult);
                LoadAuthResult();
                break;
            default:
                return $"Bad request: {res.StatusCode} {res.ReasonPhrase}";
        }
        return Success;
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
}