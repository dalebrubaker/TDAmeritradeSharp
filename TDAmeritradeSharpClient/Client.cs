using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TDAmeritradeSharpClient;

public class Client : IDisposable
{
    public const string Success = "Authorization was successful";
    private readonly ILogger<Client>? _logger;
    private readonly List<DateTime> _throttledRequestTimesUtc = new();
    private HttpClient _httpClient;

    public Client()
    {
        _logger = null;
        _httpClient = new HttpClient();
        var userSettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(TDAmeritradeSharpClient));
        if (!Directory.Exists(userSettingsDirectory))
        {
            Directory.CreateDirectory(userSettingsDirectory);
        }
        PathAuthResult = Path.Combine(userSettingsDirectory, $"{nameof(TDAmeritradeSharpClient)}.json");
        LoadAuthResult();
    }

    public Client(ILogger<Client> logger) : this()
    {
        _logger = logger;
    }

    /// <summary>
    ///     The fully-qualified path to the json file in Users that holds the Authorization information
    /// </summary>
    public string PathAuthResult { get; }

    public TDAuthResult AuthResult { get; private set; } = new();

    /// <summary>
    ///     Client has valid token
    /// </summary>
    public bool IsSignedIn => !string.IsNullOrEmpty(AuthResult.access_token);

    /// <summary>
    ///     Client has a consumer key (limited non-authenticated access)
    /// </summary>
    private bool HasConsumerKey => !string.IsNullOrEmpty(AuthResult.consumer_key);

    /// <summary>
    ///     A list of the DateTime.UtcNow when each request was made, pruned to the last minute
    /// </summary>
    public List<DateTime> ThrottledThrottledRequestTimesUtc
    {
        get
        {
            if (_throttledRequestTimesUtc.Count > 0)
            {
            }
            var firstAllowed = DateTime.UtcNow.AddMinutes(-1);
            for (var i = 0; i < _throttledRequestTimesUtc.Count; i++)
            {
                var timestamp = _throttledRequestTimesUtc[i];
                if (timestamp < firstAllowed)
                {
                    _throttledRequestTimesUtc.RemoveAt(i--);
                }
            }
            return _throttledRequestTimesUtc;
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    #region Helpers

    private static bool IsNullOrEmpty(string s)
    {
        return string.IsNullOrEmpty(s) || s == "{}";
    }

    #endregion

    #region Auth

    /// <summary>
    ///     Set the AuthResult file, returning an error message if there is an error
    /// </summary>
    /// <param name="code"></param>
    /// <param name="consumerKey"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public async Task<string> SetAuthResultsAsync(string code, string consumerKey, string callback)
    {
        var decoded = HttpUtility.UrlDecode(code);
        var dict = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "access_type", "offline" },
            { "client_id", $"{consumerKey}@AMER.OAUTHAP" },
            { "redirect_uri", callback },
            { "code", decoded }
        };
        return await AuthenticateInitAsync(code, consumerKey, callback, dict).ConfigureAwait(false);
    }

    private void LoadAuthResult()
    {
        if (!File.Exists(PathAuthResult))
        {
            return;
        }
        var json = File.ReadAllText(PathAuthResult);
        AuthResult = JsonConvert.DeserializeObject<TDAuthResult>(json);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthResult.access_token);
    }

    private void SaveAuthResult(TDAuthResult authResult)
    {
        if (authResult.refresh_token == null)
        {
            throw new Exception("Why?");
        }
        var jsonSave = JsonConvert.SerializeObject(authResult);
        File.WriteAllText(PathAuthResult, jsonSave);
    }

    /// <summary>
    ///     Get a new access token before it expires
    ///     Get a new refresh token before it expires
    /// </summary>
    public async Task RequireNotExpiredTokensAsync()
    {
        if (AuthResult.refresh_token == null)
        {
            // Not possible
            return;
        }
        if (AuthResult.AccessTokenExpirationUtc - DateTime.UtcNow < TimeSpan.FromMinutes(1))
        {
            // Check RefreshTokenExpirationUtc only when we need a new access token
            if (AuthResult.RefreshTokenExpirationUtc - DateTime.UtcNow < TimeSpan.FromDays(7))
            {
                // Need a new refresh token
                await GetNewRefreshTokenAsync().ConfigureAwait(false);
            }
            else
            {
                // Need a new access token
                await GetNewAccessTokenAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task GetNewRefreshTokenAsync()
    {
        if (AuthResult.refresh_token == null)
        {
            // Never authorized 
            throw new AuthenticationException("Not able to get a new Refresh Token. Run TDAmeritradeSharpUI to authenticate.");
        }
        var dict = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", AuthResult.refresh_token },
            { "access_type", "offline" },
            { "client_id", $"{AuthResult.consumer_key}@AMER.OAUTHAP" }
        };
        var result = await ReAuthenticateAsync(dict);
        if (result != Success)
        {
            throw new AuthenticationException($"Not able to get a new Refresh Token. {result}. Run TDAmeritradeSharpUI to authenticate.");
        }
    }

    private async Task GetNewAccessTokenAsync()
    {
        if (AuthResult.refresh_token == null)
        {
            // Never authorized 
            return;
        }
        var dict = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", AuthResult.refresh_token },
            { "client_id", $"{AuthResult.consumer_key}@AMER.OAUTHAP" }
        };
        var result = await ReAuthenticateAsync(dict);
        if (result != Success)
        {
            throw new AuthenticationException($"Not able to get a new Access Token. {result}. Run TDAmeritradeSharpUI to authenticate.");
        }
    }

    /// <summary>
    ///     Do the initial authentication
    /// </summary>
    /// <param name="code"></param>
    /// <param name="consumerKey"></param>
    /// <param name="callback"></param>
    /// <param name="dict"></param>
    /// <returns></returns>
    private async Task<string> AuthenticateInitAsync(string? code, string? consumerKey, string? callback, Dictionary<string, string> dict)
    {
        _httpClient = new HttpClient(); // remove old Auth headers
        const string Path = "https://api.tdameritrade.com/v1/oauth2/token";
        var req = new HttpRequestMessage(HttpMethod.Post, Path) { Content = new FormUrlEncodedContent(dict) };
        try
        {
            var json = await SendRequestAsync(req);
            var authResult = JsonConvert.DeserializeObject<TDAuthResult>(json) ?? new TDAuthResult();
            authResult.security_code = code;
            authResult.consumer_key = consumerKey;
            authResult.redirect_url = callback;
            authResult.CreationTimestampUtc = DateTime.UtcNow;
            SaveAuthResult(authResult);
            LoadAuthResult();
            return Success;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private async Task<string> ReAuthenticateAsync(Dictionary<string, string> dict)
    {
        _httpClient = new HttpClient(); // remove old Auth headers
        const string Path = "https://api.tdameritrade.com/v1/oauth2/token";
        var req = new HttpRequestMessage(HttpMethod.Post, Path) { Content = new FormUrlEncodedContent(dict) };
        try
        {
            var json = await SendRequestAsync(req);
            var authResult = JsonConvert.DeserializeObject<TDAuthResult>(json) ?? new TDAuthResult();
            authResult.security_code = AuthResult.security_code;
            authResult.consumer_key = AuthResult.consumer_key;
            authResult.redirect_url = AuthResult.redirect_url; // preserve old refresh token if we are only getting a new access code
            authResult.refresh_token ??= AuthResult.refresh_token;
            if (authResult.expires_in == 0)
            {
                authResult.expires_in = AuthResult.expires_in;
            }
            if (authResult.refresh_token_expires_in == 0)
            {
                authResult.refresh_token_expires_in = AuthResult.refresh_token_expires_in;
            }
            authResult.CreationTimestampUtc = DateTime.UtcNow;
            SaveAuthResult(authResult);
            LoadAuthResult();
            return Success;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    #endregion Auth

    #region Options

    /// <summary>
    ///     Get option chain for an optionable Symbol
    ///     https://developer.tdameritrade.com/option-chains/apis/get/marketdata/chains
    /// </summary>
    public async Task<TDOptionChain?> GetOptionsChainAsync(TDOptionChainRequest request)
    {
        var json = await GetOptionsChainJsonAsync(request);
        if (!IsNullOrEmpty(json))
        {
            return JsonConvert.DeserializeObject<TDOptionChain>(json, new TDOptionChainConverter());
        }
        return null;
    }

    /// <summary>
    ///     Get option chain for an optionable Symbol
    ///     https://developer.tdameritrade.com/option-chains/apis/get/marketdata/chains
    /// </summary>
    public async Task<string> GetOptionsChainJsonAsync(TDOptionChainRequest request)
    {
        if (!HasConsumerKey)
        {
            throw new Exception("ConsumerKey is null");
        }

        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (!IsSignedIn)
        {
            queryString.Add("apikey", AuthResult.consumer_key);
        }
        queryString.Add("symbol", request.symbol);
        if (request.contractType.HasValue)
        {
            queryString.Add("contractType", request.contractType.ToString());
        }
        if (request.strikeCount.HasValue)
        {
            queryString.Add("strikeCount", request.strikeCount.ToString());
        }
        queryString.Add("includeQuotes", request.includeQuotes ? "FALSE" : "TRUE");
        if (request.interval.HasValue)
        {
            queryString.Add("interval", request.interval.ToString());
        }
        if (request.strike.HasValue)
        {
            queryString.Add("strike", request.strike.Value.ToString());
        }
        if (request.fromDate.HasValue)
        {
            queryString.Add("fromDate", request.fromDate.Value.ToString("yyyy-MM-dd"));
        }
        if (request.toDate.HasValue)
        {
            queryString.Add("toDate", request.toDate.Value.ToString("yyyy-MM-dd"));
        }
        if (!string.IsNullOrEmpty(request.expMonth))
        {
            queryString.Add("expMonth", request.expMonth);
        }
        queryString.Add("optionType", request.optionType.ToString());

        if (request.strategy == TDOptionChainStrategy.ANALYTICAL)
        {
            queryString.Add("volatility", request.volatility.ToString());
            queryString.Add("underlyingPrice", request.underlyingPrice.ToString());
            queryString.Add("interestRate", request.interestRate.ToString());
            queryString.Add("daysToExpiration", request.daysToExpiration.ToString());
        }

        var q = queryString.ToString();

        var path = $"https://api.tdameritrade.com/v1/marketdata/chains?{q}";
        var result = await SendRequestAsync(path).ConfigureAwait(false);
        return result;
    }

    #endregion Options

    #region PriceHistory

    /// <summary>
    ///     Get price history for a symbol
    ///     https://developer.tdameritrade.com/price-history/apis/get/marketdata/%7Bsymbol%7D/pricehistory
    ///     https://developer.tdameritrade.com/content/price-history-samples
    /// </summary>
    public async Task<TDPriceCandle[]?> GetPriceHistoryAsync(TDPriceHistoryRequest model)
    {
        var json = await GetPriceHistoryJsonAsync(model);
        if (!IsNullOrEmpty(json))
        {
            var doc = JObject.Parse(json);
            var inner = doc["candles"].ToObject<TDPriceCandle[]>();
            return inner;
        }
        return null;
    }

    /// <summary>
    ///     Get price history for a symbol
    ///     https://developer.tdameritrade.com/price-history/apis/get/marketdata/%7Bsymbol%7D/pricehistory
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    private async Task<string> GetPriceHistoryJsonAsync(TDPriceHistoryRequest model)
    {
        if (!HasConsumerKey)
        {
            throw new Exception("ConsumerKey is null");
        }
        var key = HttpUtility.UrlEncode(AuthResult.consumer_key);
        var builder = new UriBuilder($"https://api.tdameritrade.com/v1/marketdata/{model.symbol}/pricehistory");
        var query = HttpUtility.ParseQueryString(builder.Query);
        if (!IsSignedIn)
        {
            query["apikey"] = key;
        }
        if (model.frequencyType.HasValue)
        {
            query["frequencyType"] = model.frequencyType.ToString();
            query["frequency"] = model.frequency.ToString();
        }
        if (model.endDate.HasValue)
        {
            query["endDate"] = model.endDate.Value.ToString(CultureInfo.InvariantCulture);
            query["startDate"] = model.startDate?.ToString();
        }
        if (model.periodType.HasValue)
        {
            query["periodType"] = model.periodType.ToString();
            query["period"] = model.period.ToString();
        }
        if (model.needExtendedHoursData.HasValue)
        {
            query["needExtendedHoursData"] = model.needExtendedHoursData.ToString();
        }
        builder.Query = query.ToString();
        var url = builder.ToString();
        return await SendRequestAsync(url).ConfigureAwait(false);
    }

    #endregion PriceHistory

    #region Quotes

    public Task<TDEquityQuote> GetQuote_EquityAsync(string symbol)
    {
        return GetQuoteAsync<TDEquityQuote>(symbol);
    }

    public Task<TDIndexQuote> GetQuote_IndexAsync(string symbol)
    {
        return GetQuoteAsync<TDIndexQuote>(symbol);
    }

    public Task<FutureQuote> GetQuote_FutureAsync(string symbol)
    {
        return GetQuoteAsync<FutureQuote>(symbol);
    }

    public Task<FutureOptionsQuote> GetQuote_FutureOption(string symbol)
    {
        return GetQuoteAsync<FutureOptionsQuote>(symbol);
    }

    public Task<TDOptionQuote> GetQuote_OptionAsync(string symbol)
    {
        return GetQuoteAsync<TDOptionQuote>(symbol);
    }

    public Task<TDForexQuote> GetQuote_ForexAsync(string symbol)
    {
        return GetQuoteAsync<TDForexQuote>(symbol);
    }

    /// <summary>
    ///     Get quote for a symbol
    ///     https://developer.tdameritrade.com/quotes/apis/get/marketdata/%7Bsymbol%7D/quotes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public async Task<T> GetQuoteAsync<T>(string symbol) where T : TDQuoteBase
    {
        var json = await GetQuoteJsonAsync(symbol);
        if (!IsNullOrEmpty(json))
        {
            var doc = JObject.Parse(json);
            var inner = doc.First.First as JObject;
            return inner?.ToObject<T>()!;
        }
        return null!;
    }

    /// <summary>
    ///     Get quote for a symbol
    ///     https://developer.tdameritrade.com/quotes/apis/get/marketdata/%7Bsymbol%7D/quotes
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public async Task<string> GetQuoteJsonAsync(string symbol)
    {
        if (!HasConsumerKey)
        {
            throw new Exception("ConsumerKey is null");
        }
        var key = HttpUtility.UrlEncode(AuthResult.consumer_key);
        var path = IsSignedIn
            ? $"https://api.tdameritrade.com/v1/marketdata/{symbol}/quotes"
            : $"https://api.tdameritrade.com/v1/marketdata/{symbol}/quotes?apikey={key}";

        var json = await SendRequestAsync(path).ConfigureAwait(false);
        return json;
    }

    #endregion Quotes

    #region UserInfo

    /// <summary>
    ///     User Principal details.
    ///     <param name="fields">A comma separated String which allows one to specify additional fields to return. None of these fields are returned by default.</param>
    ///     <returns></returns>
    /// </summary>
    public async Task<TDPrincipal> GetPrincipalsAsync(params TDPrincipalsFields[] fields)
    {
        var json = await GetPrincipalsJsonAsync(fields);
        if (!IsNullOrEmpty(json))
        {
            return JsonConvert.DeserializeObject<TDPrincipal>(json);
        }
        return null!;
    }

    /// <summary>
    ///     User Principal details.
    /// </summary>
    /// <param name="fields">A comma separated String which allows one to specify additional fields to return. None of these fields are returned by default.</param>
    /// <returns></returns>
    private async Task<string> GetPrincipalsJsonAsync(params TDPrincipalsFields[] fields)
    {
        if (!IsSignedIn)
        {
            throw new Exception("Not authenticated");
        }
        var arg = string.Join(",", fields.Select(o => o.ToString()));
        var path = $"https://api.tdameritrade.com/v1/userprincipals?fields={arg}";
        return await SendRequestAsync(path).ConfigureAwait(false);
    }

    #endregion UserInfo

    #region Misc

    /// <summary>
    ///     Retrieve market hours for specified single market
    /// </summary>
    public async Task<TDMarketHour> GetMarketHoursAsync(MarketTypes type, DateTime day)
    {
        var json = await GetMarketHoursJsonAsync(type, day);
        if (!IsNullOrEmpty(json))
        {
            var doc = JObject.Parse(json);
            return doc.First.First.First.First.ToObject<TDMarketHour>();
        }
        return null!;
    }

    /// <summary>
    ///     Retrieve market hours for specified single market
    /// </summary>
    public async Task<string> GetMarketHoursJsonAsync(MarketTypes type, DateTime day)
    {
        if (!HasConsumerKey)
        {
            throw new Exception("ConsumerKey is null");
        }
        var key = HttpUtility.UrlEncode(AuthResult.consumer_key);
        var dayString = day.ToString("yyyy-MM-dd").Replace("/", "-");
        var path = IsSignedIn
            ? $"https://api.tdameritrade.com/v1/marketdata/{type}/hours?date={dayString}"
            : $"https://api.tdameritrade.com/v1/marketdata/{type}/hours?apikey={key}&date={dayString}";

        return await SendRequestAsync(path).ConfigureAwait(false);
    }

    /// <summary>
    ///     Personal application will be throttled to 0-120 POST/PUT/DELETE Order requests per minute per account
    ///     based on the properties of the application you specified during the application registration process.
    ///     GET order requests will be unthrottled.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<string> SendRequestAsync(string path)
    {
        ThrottledThrottledRequestTimesUtc.Add(DateTime.UtcNow);
        var res = await _httpClient.GetAsync(path);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                return await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            default:
                throw new Exception($"Bad request: {res.StatusCode} {res.ReasonPhrase}");
        }
    }

    /// <summary>
    ///     Personal application will be throttled to 0-120 POST/PUT/DELETE Order requests per minute per account
    ///     based on the properties of the application you specified during the application registration process.
    ///     GET order requests will be unthrottled.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<string> SendRequestAsync(HttpRequestMessage req)
    {
        ThrottledThrottledRequestTimesUtc.Add(DateTime.UtcNow);
        var res = await _httpClient.SendAsync(req);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                return await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            default:
                throw new Exception($"Bad request: {res.StatusCode} {res.ReasonPhrase}");
        }
    }

    #endregion Misc

    #region Accounts

    public async Task<TDAccountModel> GetAccountAsync(string testAccount)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts//{testAccount}";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        var account = JsonConvert.DeserializeObject<TDAccountModel>(json);
        return account;
    }

    public async Task<IEnumerable<TDAccountModel>> GetAccountsAsync()
    {
        var path = "https://api.tdameritrade.com/v1/accounts";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        var accounts = JsonConvert.DeserializeObject<IEnumerable<TDAccountModel>>(json);
        return accounts;
    }

    #endregion Accounts

    #region Orders

    public async Task CancelOrderAsync(string accountId, string orderId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders/{orderId}";
        ThrottledThrottledRequestTimesUtc.Add(DateTime.UtcNow);
        var res = await _httpClient.DeleteAsync(path);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                break;
            default:
                throw new Exception($"Bad request: {res.StatusCode} {res.ReasonPhrase}");
        }
    }

    /// <summary>
    /// Places an order and returns its orderId
    /// </summary>
    /// <param name="order"></param>
    /// <param name="accountId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> PlaceOrderAsync(OrderBase order, string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders";
        var json = order.GetJson();
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Personal application will be throttled to 0-120 POST/PUT/DELETE Order requests per minute per account based on the properties of the application you specified during the application registration process.
        // GET order requests will be unthrottled.
        ThrottledThrottledRequestTimesUtc.Add(DateTime.UtcNow);
        var httpResponseMessage = await _httpClient.PostAsync(path, content);
        switch (httpResponseMessage.StatusCode)
        {
            case HttpStatusCode.Created:
                var location = httpResponseMessage.Headers.First(x => x.Key == "Location");
                var value = location.Value.First();
                var lastSlashIndex = value.LastIndexOf("/", StringComparison.Ordinal);
                var orderId = value.Substring(lastSlashIndex + 1, value.Length - lastSlashIndex - 1);
                return orderId;
            default:
                throw new Exception($"Bad request: {httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}");
        }
    }

    public async Task<IEnumerable<TDOrderResponse>> GetOrdersForAccountAsync(string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        var result0 = JsonConvert.DeserializeObject(json);
        try
        {
            var result = JsonConvert.DeserializeObject<IEnumerable<TDOrderResponse>>(json);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion Orders

    public async Task<TDOrderResponse> GetOrderAsync(string accountId, string orderId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders/{orderId}";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        var result = JsonConvert.DeserializeObject<TDOrderResponse>(json);
        return result;
    }
}