using System.Net;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TDAmeritradeSharpClient
{
    public class Client : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Client> _logger;
        private readonly string _pathAuthResult;
        private AuthResult? _authResult;

        public Client(ILogger<Client> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            var userSettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(TDAmeritradeSharpClient));
            if (!Directory.Exists(userSettingsDirectory))
            {
                Directory.CreateDirectory(userSettingsDirectory);
            }
            _pathAuthResult = Path.Combine(userSettingsDirectory, $"{nameof(TDAmeritradeSharpClient)}.json");
            LoadAuthResult();
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
            using var client = new HttpClient();
            var dict = new Dictionary<string, string> {
                { "grant_type", "authorization_code" }, { "access_type", "offline" }, { "client_id", $"{consumerKey}@AMER.OAUTHAP" }, { "redirect_uri", callback }, { "code", decoded }
            };
            var req = new HttpRequestMessage(HttpMethod.Post, path) { Content = new FormUrlEncodedContent(dict) };
            var res = await client.SendAsync(req);
            switch (res.StatusCode)
            {
                case HttpStatusCode.OK:
                    var json = await res.Content.ReadAsStringAsync();
                    var authResult = JsonConvert.DeserializeObject<AuthResult>(json) ?? new AuthResult();
                    authResult.SecurityCode = code;
                    authResult.ConsumerKey = consumerKey;
                    authResult.RedirectUrl = callback;
                    SaveAuthResult(authResult);
                    LoadAuthResult();
                    break;
                default:
                    return $"Bad request: {res.StatusCode} {res.ReasonPhrase}";
            }
            return "";
        }

        private void LoadAuthResult()
        {
            if (!File.Exists(_pathAuthResult))
            {
                return;
            }
            var json = File.ReadAllText(_pathAuthResult);
            _authResult = JsonConvert.DeserializeObject<AuthResult>(json);
        }

        private void SaveAuthResult(AuthResult authResult)
        {
            var jsonSave = JsonConvert.SerializeObject(authResult);
            File.WriteAllText(_pathAuthResult, jsonSave);
        }
    }
}