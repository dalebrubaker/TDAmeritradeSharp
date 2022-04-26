using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Web;

namespace TDAmeritradeSharpClient;

public class Client : IDisposable
{
    public const string Success = "Authorization was successful";
    private HttpClient _httpClient;

    public Client()
    {
        var options = new JsonSerializerOptions();

        _httpClient = new HttpClient();
        var userSettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(TDAmeritradeSharpClient));
        if (!Directory.Exists(userSettingsDirectory))
        {
            Directory.CreateDirectory(userSettingsDirectory);
        }
        PathAuthValues = Path.Combine(userSettingsDirectory, $"{nameof(TDAmeritradeSharpClient)}.json");
        LoadAuthResult();
    }

    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    ///     The fully-qualified path to the json file in Users that holds the Authorization information
    /// </summary>
    public string PathAuthValues { get; }

    public TDAuthValues? AuthValues { get; private set; }

    /// <summary>
    ///     Client has valid token
    /// </summary>
    public bool IsSignedIn => AuthValues != null && !string.IsNullOrEmpty(AuthValues.AccessToken);

    /// <summary>
    ///     Client has a consumer key (limited non-authenticated access)
    /// </summary>
    private bool HasConsumerKey => AuthValues != null && !string.IsNullOrEmpty(AuthValues.ConsumerKey);

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
    public async Task<string> InitializeAuthValuesAsync(string code, string consumerKey, string callback)
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
        _httpClient = new HttpClient(); // remove old Auth headers
        const string Path = "https://api.tdameritrade.com/v1/oauth2/token";
        var req = new HttpRequestMessage(HttpMethod.Post, Path) { Content = new FormUrlEncodedContent(dict) };
        try
        {
            var json = await SendRequestAsync(req);
            var authResponse = JsonSerializer.Deserialize<TDAuthResponse>(json);
            AuthValues = new TDAuthValues(callback!, consumerKey!, authResponse ?? throw new InvalidOperationException());
            SaveAuthResult(AuthValues);
            LoadAuthResult();
            return Success;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private void LoadAuthResult()
    {
        if (!File.Exists(PathAuthValues))
        {
            return;
        }
        var json = File.ReadAllText(PathAuthValues);
        AuthValues = JsonSerializer.Deserialize<TDAuthValues>(json);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthValues?.AccessToken);
    }

    private void SaveAuthResult(TDAuthValues authValues)
    {
        if (authValues.RefreshToken == null)
        {
            throw new Exception("Why?");
        }
        var jsonSave = JsonSerializer.Serialize(authValues);
        File.WriteAllText(PathAuthValues, jsonSave);
    }

    /// <summary>
    ///     Get a new access token before it expires
    ///     Get a new refresh token before it expires
    /// </summary>
    public async Task RequireNotExpiredTokensAsync()
    {
        if (AuthValues == null)
        {
            throw new NoNullAllowedException($"AuthValues is null. Must call {nameof(InitializeAuthValuesAsync)}");
        }
        if (AuthValues.AccessTokenExpirationUtc - DateTime.UtcNow < TimeSpan.FromMinutes(1))
        {
            // Check RefreshTokenExpirationUtc only when we need a new access token
            if (AuthValues.RefreshTokenExpirationUtc - DateTime.UtcNow < TimeSpan.FromDays(7))
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

    /// <summary>
    ///     internal for Tests project
    /// </summary>
    /// <exception cref="AuthenticationException"></exception>
    internal async Task GetNewAccessTokenAsync()
    {
        if (AuthValues == null)
        {
            throw new NoNullAllowedException($"AuthValues is null. Must call {nameof(InitializeAuthValuesAsync)}");
        }
        var dict = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", AuthValues.RefreshToken },
            { "client_id", $"{AuthValues.ConsumerKey}@AMER.OAUTHAP" }
        };
        _httpClient = new HttpClient(); // remove old Auth headers
        const string Path = "https://api.tdameritrade.com/v1/oauth2/token";
        var req = new HttpRequestMessage(HttpMethod.Post, Path) { Content = new FormUrlEncodedContent(dict) };
        try
        {
            var json = await SendRequestAsync(req);
            var authResponse = JsonSerializer.Deserialize<TDAuthResponse>(json);
            Debug.Assert(authResponse!.refresh_token == null, "Refresh token is not returned.");
            Debug.Assert(authResponse.refresh_token_expires_in == 0);
            Debug.Assert(authResponse.access_token != null);
            Debug.Assert(authResponse.expires_in > 0);
            AuthValues.AccessToken = authResponse.access_token!;
            AuthValues.AccessTokenExpirationUtc = DateTime.UtcNow.AddSeconds(authResponse.expires_in);
            SaveAuthResult(AuthValues);
            LoadAuthResult();
        }
        catch (Exception ex)
        {
            throw new AuthenticationException($"Not able to get a new Access Token. {ex.Message}. Run TDAmeritradeSharpUI to authenticate.");
        }
    }

    /// <summary>
    ///     internal for Tests project
    /// </summary>
    /// <exception cref="AuthenticationException"></exception>
    internal async Task GetNewRefreshTokenAsync()
    {
        if (AuthValues == null)
        {
            // Never authorized 
            throw new AuthenticationException("Not able to get a new Refresh Token. Run TDAmeritradeSharpUI to authenticate.");
        }
        var dict = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", AuthValues.RefreshToken },
            { "access_type", "offline" },
            { "client_id", $"{AuthValues.ConsumerKey}@AMER.OAUTHAP" }
        };
        _httpClient = new HttpClient(); // remove old Auth headers
        const string Path = "https://api.tdameritrade.com/v1/oauth2/token";
        var req = new HttpRequestMessage(HttpMethod.Post, Path) { Content = new FormUrlEncodedContent(dict) };
        try
        {
            var json = await SendRequestAsync(req);
            var authResponse = JsonSerializer.Deserialize<TDAuthResponse>(json);
            Debug.Assert(authResponse!.refresh_token != null);
            Debug.Assert(authResponse.refresh_token_expires_in > 0);
            AuthValues.RefreshToken = authResponse.refresh_token!;
            AuthValues.RefreshTokenExpirationUtc = DateTime.UtcNow.AddSeconds(authResponse.refresh_token_expires_in);

            // We also got a new AccessToken
            Debug.Assert(authResponse.access_token != null);
            Debug.Assert(authResponse.expires_in > 0);
            AuthValues.AccessToken = authResponse.access_token!;
            AuthValues.AccessTokenExpirationUtc = DateTime.UtcNow.AddSeconds(authResponse.expires_in);
            SaveAuthResult(AuthValues);
            LoadAuthResult();
        }
        catch (Exception ex)
        {
            throw new AuthenticationException($"Not able to get a new Refresh Token. {ex.Message}. Run TDAmeritradeSharpUI to authenticate.");
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
        return !IsNullOrEmpty(json) ? JsonSerializer.Deserialize<TDOptionChain>(json) : null;
    }

    /// <summary>
    ///     Get option chain for an optionable Symbol
    ///     https://developer.tdameritrade.com/option-chains/apis/get/marketdata/chains
    /// </summary>
    public async Task<string> GetOptionsChainJsonAsync(TDOptionChainRequest request)
    {
        if (AuthValues == null)
        {
            throw new NoNullAllowedException($"AuthValues is null. Must call {nameof(InitializeAuthValuesAsync)}");
        }
        if (!HasConsumerKey)
        {
            throw new Exception("ConsumerKey is null");
        }
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (!IsSignedIn)
        {
            queryString.Add("apikey", AuthValues.ConsumerKey);
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
        // if (!IsNullOrEmpty(json))
        // {
        //     var doc = JObject.Parse(json);
        //     var inner = doc["candles"].ToObject<TDPriceCandle[]>();
        //     return inner;
        // }
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
        if (AuthValues == null)
        {
            throw new NoNullAllowedException($"AuthValues is null. Must call {nameof(InitializeAuthValuesAsync)}");
        }
        if (!HasConsumerKey)
        {
            throw new Exception("ConsumerKey is null");
        }
        var key = HttpUtility.UrlEncode(AuthValues.ConsumerKey);
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
            // TODO
            // var doc = JObject.Parse(json);
            // var inner = doc.First.First as JObject;
            // return inner?.ToObject<T>()!;
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
        if (AuthValues == null)
        {
            throw new NoNullAllowedException($"AuthValues is null. Must call {nameof(InitializeAuthValuesAsync)}");
        }
        if (!HasConsumerKey)
        {
            throw new Exception("ConsumerKey is null");
        }
        var key = HttpUtility.UrlEncode(AuthValues.ConsumerKey);
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
    public async Task<TDPrincipal> GetUserPrincipalsAsync(params TDPrincipalsFields[] fields)
    {
        var json = await GetUserPrincipalsJsonAsync(fields);
        return (!IsNullOrEmpty(json) ? JsonSerializer.Deserialize<TDPrincipal>(json) : null!) ?? throw new InvalidOperationException();
    }

    /// <summary>
    ///     Return account information for accountId, including the display name, or <c>null</c> if not found.
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public async Task<TDAccount?> GetAccountPrincipalInfoAsync(string accountId)
    {
        var data = await GetUserPrincipalsAsync(TDPrincipalsFields.preferences); // gives Accounts including display names    }
        var account = (data.accounts ?? throw new InvalidOperationException()).FirstOrDefault(x => x.accountId == accountId);
        return account;
    }

    /// <summary>
    ///     User Principal details.
    /// </summary>
    /// <param name="fields">A comma separated String which allows one to specify additional fields to return. None of these fields are returned by default.</param>
    /// <returns></returns>
    private async Task<string> GetUserPrincipalsJsonAsync(params TDPrincipalsFields[] fields)
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
    public async Task<TDMarketHours> GetHoursForASingleMarketAsync(MarketTypes marketType, DateTime day)
    {
        if (AuthValues == null)
        {
            throw new NoNullAllowedException($"AuthValues is null. Must call {nameof(InitializeAuthValuesAsync)}");
        }
        if (!HasConsumerKey)
        {
            throw new Exception("ConsumerKey is null");
        }
        var key = HttpUtility.UrlEncode(AuthValues.ConsumerKey);
        var dayString = day.ToString("yyyy'-'MM'-'dd");
        var path = IsSignedIn
            ? $"https://api.tdameritrade.com/v1/marketdata/{marketType}/hours?date={dayString}"
            : $"https://api.tdameritrade.com/v1/marketdata/{marketType}/hours?apikey={key}&date={dayString}";

        var json = await SendRequestAsync(path).ConfigureAwait(false);
        if (IsNullOrEmpty(json))
        {
            return null!;
        }
        var result = GetMarketHours(marketType, json);
        return result ?? throw new InvalidOperationException();
    }

    private static TDMarketHours? GetMarketHours(MarketTypes marketType, string json)
    {
        var node = JsonNode.Parse(json);
        if (node == null)
        {
            throw new InvalidOperationException();
        }
        var jsonHours = "";
        switch (marketType)
        {
            case MarketTypes.BOND:
                break;
            case MarketTypes.EQUITY:
                jsonHours = node["equity"]?["EQ"]?.ToJsonString();
                break;
            case MarketTypes.ETF:
                break;
            case MarketTypes.FOREX:
                break;
            case MarketTypes.FUTURE:
                break;
            case MarketTypes.FUTURE_OPTION:
                break;
            case MarketTypes.INDEX:
                break;
            case MarketTypes.INDICAT:
                break;
            case MarketTypes.MUTUAL_FUND:
                break;
            case MarketTypes.OPTION:
                jsonHours = node["option"]?["EQO"]?.ToJsonString();
                break;
            case MarketTypes.UNKNOWN:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(marketType), marketType, null);
        }
        var result = JsonSerializer.Deserialize<TDMarketHours>(jsonHours ?? throw new InvalidOperationException(), JsonOptions);
        return result;
    }

    private async Task<string> SendRequestAsync(string path)
    {
        var res = await _httpClient.GetAsync(path);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                return await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            default:
                throw new Exception($"Bad request: {res.StatusCode} {res.ReasonPhrase}");
        }
    }

    private async Task<string> SendRequestAsync(HttpRequestMessage req)
    {
        var res = await _httpClient.Throttle().SendAsync(req);
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
        var account = JsonSerializer.Deserialize<TDAccountModel>(json);
        return account ?? throw new InvalidOperationException();
    }

    public async Task<IEnumerable<TDAccountModel>> GetAccountsAsync()
    {
        var path = "https://api.tdameritrade.com/v1/accounts";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        //var accountsTmp = JsonSerializer.Deserialize(json);
        var accounts = JsonSerializer.Deserialize<IEnumerable<TDAccountModel>>(json);
        Debug.Assert(accounts != null, nameof(accounts) + " != null");
        return accounts ?? throw new InvalidOperationException();
    }

    #endregion Accounts

    #region Orders

    public async Task<TDOrderResponse> GetOrderAsync(string accountId, string orderId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders/{orderId}";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        var result = JsonSerializer.Deserialize<TDOrderResponse>(json);
        return result ?? throw new InvalidOperationException();
    }

    /// <summary>
    ///     Do GetOrdersByPath
    /// </summary>
    /// <param name="accountId">the account Id</param>
    /// <param name="maxResults">
    ///     The maximum number of orders to retrieve. <c>null</c> means "all".
    ///     Bug: 1 gives 1 but greater numbers give maxResults - 1 IFF status is <c>null</c>
    /// </param>
    /// <param name="fromEnteredTime">Specifies that no orders entered before this time should be returned. <c>null</c> means to start with the current day.</param>
    /// <param name="toEnteredTime">Specifies that no orders entered after this time should be returned. <c>null</c> means to end 60 days after toEnteredTime.</param>
    /// <param name="status">Specifies that only orders of this status should be returned. <c>null</c> means "all".</param>
    /// <returns>The list of orders matching this query.</returns>
    public async Task<IEnumerable<TDOrderResponse>> GetOrdersByPathAsync(string accountId, int? maxResults = null, DateTime? fromEnteredTime = null,
        DateTime? toEnteredTime = null, TDOrderModelsEnums.status? status = null)
    {
        // Add queryString /orders?maxResults=1&status=CANCELED" Dates are yyyy-mm-dd if not null
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (maxResults.HasValue)
        {
            queryString.Add("maxResults", maxResults.ToString());
        }
        if (fromEnteredTime.HasValue)
        {
            queryString.Add("fromEnteredTime", $"{fromEnteredTime:yyyy-MM-dd}");
        }
        if (toEnteredTime.HasValue)
        {
            queryString.Add("toEnteredTime", $"{toEnteredTime:yyyy-MM-dd}");
        }
        if (status.HasValue)
        {
            queryString.Add("status", $"{status.ToString()}");
        }
        var q = queryString.ToString();
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders?{q}";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        try
        {
            //var result0 = JsonSerializer.Deserialize(json);
            var result = JsonSerializer.Deserialize<IEnumerable<TDOrderResponse>>(json);
            return result ?? throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    ///     Do GetOrdersByQuery
    /// </summary>
    /// <param name="accountId">the account Id. <c>null</c> means "all".</param>
    /// <param name="maxResults">
    ///     The maximum number of orders to retrieve. <c>null</c> means "all".
    ///     Bug: 1 gives 1 but greater numbers give maxResults - 1 IFF status is <c>null</c>
    /// </param>
    /// <param name="fromEnteredTime">Specifies that no orders entered before this time should be returned. <c>null</c> means to start with the current day.</param>
    /// <param name="toEnteredTime">Specifies that no orders entered after this time should be returned. <c>null</c> means to end 60 days after toEnteredTime.</param>
    /// <param name="status">
    ///     Specifies that only orders of this status should be returned. <c>null</c> means "all".<</param>
    /// <returns>The list of orders matching this query.</returns>
    public async Task<IEnumerable<TDOrderResponse>> GetOrdersByQueryAsync(string? accountId = null, int? maxResults = null, DateTime? fromEnteredTime = null,
        DateTime? toEnteredTime = null, TDOrderModelsEnums.status? status = null)
    {
        // Add queryString /orders?maxResults=1&status=CANCELED" Dates are yyyy-mm-dd if not null
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        if (accountId != null)
        {
            queryString.Add("accountId", accountId);
        }
        if (maxResults.HasValue)
        {
            queryString.Add("maxResults", maxResults.ToString());
        }
        if (fromEnteredTime.HasValue)
        {
            queryString.Add("fromEnteredTime", $"{fromEnteredTime:yyyy-MM-dd}");
        }
        if (toEnteredTime.HasValue)
        {
            queryString.Add("toEnteredTime", $"{toEnteredTime:yyyy-MM-dd}");
        }
        if (status.HasValue)
        {
            queryString.Add("status", $"{status.ToString()}");
        }
        var q = queryString.ToString();
        var path = $"https://api.tdameritrade.com/v1/orders?{q}";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        try
        {
            //var result0 = JsonSerializer.Deserialize(json);
            var result = JsonSerializer.Deserialize<IEnumerable<TDOrderResponse>>(json);
            return result ?? throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task CancelOrderAsync(string accountId, string orderId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders/{orderId}";
        var res = await _httpClient.Throttle().DeleteAsync(path);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                break;
            default:
                var existingOrder = await GetOrderAsync(accountId, orderId);
                var status = existingOrder.status;
                throw new Exception($"Bad request: {res.StatusCode} {res.ReasonPhrase}");
        }
    }

    /// <summary>
    ///     Places an order and returns its orderId
    /// </summary>
    /// <param name="order">The order to be placed.</param>
    /// <param name="accountId">The accountId</param>
    /// <returns>the orderId assigned by TDAmeritrade to this order.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> PlaceOrderAsync(OrderBase order, string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders";
        var json = order.GetJson();
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponseMessage = await _httpClient.Throttle().PostAsync(path, content);
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

    /// <summary>
    ///     Places an order and returns its orderId
    /// </summary>
    /// <param name="order">The order to be placed.</param>
    /// <param name="accountId">The accountId</param>
    /// <returns>the orderId assigned by TDAmeritrade to this order.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> PlaceOcoOrderAsync(OcoOrder order, string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders";
        var json = order.GetJson();
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponseMessage = await _httpClient.Throttle().PostAsync(path, content);
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

    /// <summary>
    ///     Replace an existing order with replacementOrder
    /// </summary>
    /// <param name="replacementOrder">The order to replace the order identified by orderId</param>
    /// <param name="accountId">The accountId</param>
    /// <param name="orderId">The orderId of the order to replace.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> ReplaceOrderAsync(OrderBase replacementOrder, string accountId, string orderId)
    {
        var json = replacementOrder.GetJson();
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders/{orderId}";
        var httpResponseMessage = await _httpClient.Throttle().PutAsync(path, content);
        switch (httpResponseMessage.StatusCode)
        {
            case HttpStatusCode.Created:
                var location = httpResponseMessage.Headers.First(x => x.Key == "Location");
                var value = location.Value.First();
                var lastSlashIndex = value.LastIndexOf("/", StringComparison.Ordinal);
                var replacementOrderId = value.Substring(lastSlashIndex + 1, value.Length - lastSlashIndex - 1);
                return replacementOrderId;
            default:
                throw new Exception($"Bad request: {httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}");
        }
    }

    /// <summary>
    ///     Places an order and returns its orderId
    /// </summary>
    /// <param name="order">The order to be placed.</param>
    /// <param name="accountId">The accountId</param>
    /// <returns>the orderId assigned by TDAmeritrade to this order.</returns>
    /// <exception cref="Exception"></exception>
    public async Task CreateSavedOrderAsync(OrderBase order, string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/savedorders";
        var json = order.GetJson();
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponseMessage = await _httpClient.Throttle().PostAsync(path, content);
        switch (httpResponseMessage.StatusCode)
        {
            case HttpStatusCode.OK:
                break;
            default:
                throw new Exception($"Bad request: {httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}");
        }
    }

    public async Task<TDOrderResponse> GetSavedOrderAsync(string accountId, string savedOrderId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/savedorders/{savedOrderId}";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        var result = JsonSerializer.Deserialize<TDOrderResponse>(json);
        return result ?? throw new InvalidOperationException();
    }

    public async Task<IEnumerable<TDOrderResponse>> GetSavedOrdersByPathAsync(string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/savedorders";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        try
        {
            var result = JsonSerializer.Deserialize<IEnumerable<TDOrderResponse>>(json);
            return result ?? throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task DeleteSavedOrderAsync(string accountId, string savedOrderId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/savedorders/{savedOrderId}";
        var res = await _httpClient.Throttle().DeleteAsync(path);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                break;
            default:
                // var existingOrder = await GetOrderAsync(accountId, savedOrderId);
                // var status = existingOrder.status;
                throw new Exception($"Bad request: {res.StatusCode} {res.ReasonPhrase}");
        }
    }

    /// <summary>
    ///     Replace an existing order with replacementOrder
    /// </summary>
    /// <param name="replacementOrder">The order to replace the order identified by savedOrderId</param>
    /// <param name="accountId">The accountId</param>
    /// <param name="savedOrderId">The savedOrderId of the order to replace.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task ReplaceSavedOrderAsync(OrderBase replacementOrder, string accountId, string savedOrderId)
    {
        var json = replacementOrder.GetJson();
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/savedorders/{savedOrderId}";
        var httpResponseMessage = await _httpClient.Throttle().PutAsync(path, content);
        switch (httpResponseMessage.StatusCode)
        {
            case HttpStatusCode.OK:
                break;
            default:
                throw new Exception($"Bad request: {httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}");
        }
    }

    #endregion Orders
}