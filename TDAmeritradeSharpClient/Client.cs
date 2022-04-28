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
    private readonly JsonSerializerOptions _jsonOptionsWithoutPolymorphicConverters;
    private HttpClient _httpClient;

    public Client()
    {
        _httpClient = new HttpClient();
        JsonOptions = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            Converters =
            {
                new StringConverter(),
                new TDOptionChainConverter(),
                new TDInstrumentConverter(),
                new TDAccountConverter()
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        _jsonOptionsWithoutPolymorphicConverters = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            Converters =
            {
                new StringConverter(),
                new TDOptionChainConverter()
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        var userSettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(TDAmeritradeSharpClient));
        if (!Directory.Exists(userSettingsDirectory))
        {
            Directory.CreateDirectory(userSettingsDirectory);
        }
        PathAuthValues = Path.Combine(userSettingsDirectory, $"{nameof(TDAmeritradeSharpClient)}.json");
        LoadAuthResult();
    }

    public JsonSerializerOptions JsonOptions { get; }

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

    /// <summary>
    ///     Use this to correctly serialize an instrument of any type
    ///     TDA Instruments are polymorphic.
    /// </summary>
    /// <param name="instrument"></param>
    /// <returns></returns>
    public string SerializeInstrument(TDInstrument instrument)
    {
        var json = JsonSerializer.Serialize(instrument, instrument.GetType(), _jsonOptionsWithoutPolymorphicConverters);
        return json;
    }
    
    /// <summary>
    ///     Use this to correctly deserialize to an instrument of the correct type.
    ///     TDA Instruments are polymorphic.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public TDInstrument? DeserializeToInstrument(string json)
    {
        var instrument = JsonSerializer.Deserialize<TDInstrument>(json, JsonOptions);
        return instrument;
    }

    /// <summary>
    ///     Use this to correctly serialize an account of any type
    ///     TDA Accounts are polymorphic.
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public string SerializeAccount(SecuritiesAccount account)
    {
        var json = JsonSerializer.Serialize(account, account.GetType(), _jsonOptionsWithoutPolymorphicConverters);
        return json;
    }

    /// <summary>
    ///     Use this to correctly deserialize to an account of the correct type.
    ///     TDA Accounts are polymorphic.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public SecuritiesAccount? DeserializeToAccount(string json)
    {
        var account = JsonSerializer.Deserialize<SecuritiesAccount>(json, JsonOptions);
        return account;
    }

    /// <summary>
    ///     Return a deep clone of this order
    /// </summary>
    /// <returns></returns>
    public TDOrder CloneDeep(TDOrder order)
    {
        var json = JsonSerializer.Serialize(order, _jsonOptionsWithoutPolymorphicConverters);
        var clone = JsonSerializer.Deserialize<TDOrder>(json, JsonOptions);
        return clone!;
    }

    /// <summary>
    ///     Serialize the order without using <see cref="TDInstrumentConverter" />, which supports deserialization but not serialization
    ///     (because System.Text.Json chose not to handle polymorphic deserialization).
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    /// <exception cref="TDAmeritradeSharpException"></exception>
    public string SerializeOrder(TDOrder order)
    {
        var json = JsonSerializer.Serialize(order, _jsonOptionsWithoutPolymorphicConverters);
        if (json == "null")
        {
            throw new TDAmeritradeSharpException();
        }
        return json;
    }

    #endregion Helpers

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
            AuthValues = new TDAuthValues(callback!, consumerKey!, authResponse ?? throw new TDAmeritradeSharpException());
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
        if (IsNullOrEmpty(json))
        {
            throw new TDAmeritradeSharpException();
        }
        var result = JsonSerializer.Deserialize<TDOptionChain>(json, JsonOptions);
        return result;
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
        queryString.Add("symbol", request.Symbol);
        if (request.ContractType.HasValue)
        {
            queryString.Add("contractType", request.ContractType.ToString());
        }
        if (request.StrikeCount.HasValue)
        {
            queryString.Add("strikeCount", request.StrikeCount.ToString());
        }
        queryString.Add("includeQuotes", request.IncludeQuotes ? "FALSE" : "TRUE");
        if (request.Interval.HasValue)
        {
            queryString.Add("interval", request.Interval.ToString());
        }
        if (request.Strike.HasValue)
        {
            queryString.Add("strike", request.Strike.Value.ToString());
        }
        if (request.FromDate.HasValue)
        {
            queryString.Add("fromDate", request.FromDate.Value.ToString("yyyy-MM-dd"));
        }
        if (request.ToDate.HasValue)
        {
            var str = request.ToDate.Value.ToString("yyyy-MM-dd");
            queryString.Add("toDate", str);
        }
        if (!string.IsNullOrEmpty(request.ExpMonth))
        {
            queryString.Add("expMonth", request.ExpMonth);
        }
        queryString.Add("optionType", request.OptionType.ToString());

        if (request.Strategy == TDOptionChainStrategy.ANALYTICAL)
        {
            queryString.Add("volatility", request.Volatility.ToString());
            queryString.Add("underlyingPrice", request.UnderlyingPrice.ToString());
            queryString.Add("interestRate", request.InterestRate.ToString());
            queryString.Add("daysToExpiration", request.DaysToExpiration.ToString());
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
        if (IsNullOrEmpty(json))
        {
            return null;
        }
        var node = JsonNode.Parse(json);
        var candles = node?["candles"].Deserialize<TDPriceCandle[]>(JsonOptions);
        return candles;
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
        var builder = new UriBuilder($"https://api.tdameritrade.com/v1/marketdata/{model.Symbol}/pricehistory");
        var query = HttpUtility.ParseQueryString(builder.Query);
        if (!IsSignedIn)
        {
            query["apikey"] = key;
        }
        if (model.FrequencyType.HasValue)
        {
            query["frequencyType"] = model.FrequencyType.ToString();
            query["frequency"] = model.Frequency.ToString();
        }
        if (model.EndDate.HasValue)
        {
            query["endDate"] = model.EndDate.Value.ToString(CultureInfo.InvariantCulture);
            query["startDate"] = model.StartDate?.ToString();
        }
        if (model.PeriodType.HasValue)
        {
            query["periodType"] = model.PeriodType.ToString();
            query["period"] = model.Period.ToString();
        }
        if (model.NeedExtendedHoursData.HasValue)
        {
            query["needExtendedHoursData"] = model.NeedExtendedHoursData.ToString();
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
        if (IsNullOrEmpty(json))
        {
            throw new TDAmeritradeSharpException();
        }
        var node = JsonNode.Parse(json);
        if (node == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var nodeQuote = node[symbol];
        var result = nodeQuote?.Deserialize<T>(JsonOptions);
        return result!;
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
        var symbolEncoded = HttpUtility.UrlEncode(symbol); // handle futures like /ES
        var path = IsSignedIn
            ? $"https://api.tdameritrade.com/v1/marketdata/quotes?symbol={symbolEncoded}"
            : $"https://api.tdameritrade.com/v1/marketdata/quotes?apikey={key}&symbol={symbolEncoded}";

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
        return (!IsNullOrEmpty(json) ? JsonSerializer.Deserialize<TDPrincipal>(json) : null!) ?? throw new TDAmeritradeSharpException();
    }

    /// <summary>
    ///     Return account information for accountId, including the display name, or <c>null</c> if not found.
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public async Task<TDPrincipalAccount?> GetAccountPrincipalInfoAsync(string accountId)
    {
        var data = await GetUserPrincipalsAsync(TDPrincipalsFields.preferences); // gives Accounts including display names    }
        var account = (data.accounts ?? throw new TDAmeritradeSharpException()).FirstOrDefault(x => x.accountId == accountId);
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
        return result ?? throw new TDAmeritradeSharpException();
    }

    private TDMarketHours? GetMarketHours(MarketTypes marketType, string json)
    {
        var node = JsonNode.Parse(json);
        if (node == null)
        {
            throw new TDAmeritradeSharpException();
        }
        switch (marketType)
        {
            case MarketTypes.BOND:
                return node["bond"]?["BON"]?.Deserialize<TDMarketHours>(JsonOptions) ?? throw new TDAmeritradeSharpException();
            case MarketTypes.EQUITY:
                return node["equity"]?["EQ"]?.Deserialize<TDMarketHours>(JsonOptions) ?? throw new TDAmeritradeSharpException();
            case MarketTypes.ETF:
                throw new NotSupportedException();
            case MarketTypes.FOREX:
                return node["forex"]?["forex"]?.Deserialize<TDMarketHours>(JsonOptions) ?? throw new TDAmeritradeSharpException();
            case MarketTypes.FUTURE:
                return node["future"]?["DFE"]?.Deserialize<TDMarketHours>(JsonOptions) ?? throw new TDAmeritradeSharpException();
            case MarketTypes.FUTURE_OPTION:
                throw new NotSupportedException();
            case MarketTypes.INDEX:
                throw new NotSupportedException();
            case MarketTypes.INDICAT:
                throw new NotSupportedException();
            case MarketTypes.MUTUAL_FUND:
                throw new NotSupportedException();
            case MarketTypes.OPTION:
                return node["option"]?["EQO"]?.Deserialize<TDMarketHours>(JsonOptions) ?? throw new TDAmeritradeSharpException();
            case MarketTypes.UNKNOWN:
                throw new NotSupportedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(marketType), marketType, null);
        }
        throw new TDAmeritradeSharpException();
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

    public async Task<TDAccount> GetAccountAsync(string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts//{accountId}";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        var account = JsonSerializer.Deserialize<TDAccount>(json, JsonOptions);
        return account ?? throw new TDAmeritradeSharpException();
    }

    public async Task<IEnumerable<TDAccount>> GetAccountsAsync()
    {
        var path = "https://api.tdameritrade.com/v1/accounts";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        var accounts = JsonSerializer.Deserialize<IEnumerable<TDAccount>>(json, JsonOptions);
        Debug.Assert(accounts != null, nameof(accounts) + " != null");
        return accounts ?? throw new TDAmeritradeSharpException();
    }

    #endregion Accounts

    #region Orders

    public async Task<TDOrder> GetOrderAsync(string accountId, long orderId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders/{orderId}";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        var result = JsonSerializer.Deserialize<TDOrder>(json, JsonOptions);
        return result ?? throw new TDAmeritradeSharpException();
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
    public async Task<IEnumerable<TDOrder>> GetOrdersByPathAsync(string accountId, int? maxResults = null, DateTime? fromEnteredTime = null,
        DateTime? toEnteredTime = null, TDOrderEnums.Status? status = null)
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
            var result = JsonSerializer.Deserialize<IEnumerable<TDOrder>>(json, JsonOptions);
            return result ?? throw new TDAmeritradeSharpException();
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
    public async Task<IEnumerable<TDOrder>> GetOrdersByQueryAsync(string? accountId = null, int? maxResults = null, DateTime? fromEnteredTime = null,
        DateTime? toEnteredTime = null, TDOrderEnums.Status? status = null)
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
            var result = JsonSerializer.Deserialize<IEnumerable<TDOrder>>(json, JsonOptions);
            return result ?? throw new TDAmeritradeSharpException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task CancelOrderAsync(string accountId, long orderId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders/{orderId}";
        var res = await _httpClient.Throttle().DeleteAsync(path);
        switch (res.StatusCode)
        {
            case HttpStatusCode.OK:
                break;
            default:
                var existingOrder = await GetOrderAsync(accountId, orderId);
                var status = existingOrder.Status;
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
    public async Task<long> PlaceOrderAsync(TDOrder order, string accountId)
    {
        var json = SerializeOrder(order);
        return await PlaceOrderAsync(json, accountId);
    }

    /// <summary>
    ///     This method allows you to send the raw json that you would enter manually at https://developer.tdameritrade.com/account-access/apis/post/accounts/%7BaccountId%7D/orders-0
    ///     The TD Ameritrade API will give you errors feedback there that is not available here in the http response.
    /// </summary>
    /// <param name="json"></param>
    /// <param name="accountId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<long> PlaceOrderAsync(string json, string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders";
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponseMessage = await _httpClient.Throttle().PostAsync(path, content);
        switch (httpResponseMessage.StatusCode)
        {
            case HttpStatusCode.Created:
                var location = httpResponseMessage.Headers.First(x => x.Key == "Location");
                var value = location.Value.First();
                var lastSlashIndex = value.LastIndexOf("/", StringComparison.Ordinal);
                var orderIdStr = value.Substring(lastSlashIndex + 1, value.Length - lastSlashIndex - 1);
                long.TryParse(orderIdStr, out var orderId);
                return orderId;
            default:
                throw new Exception($"Bad request: {httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}");
        }
    }

    /// <summary>
    ///     Places an oco order and returns its orderId
    /// </summary>
    /// <param name="order">The order to be placed.</param>
    /// <param name="accountId">The accountId</param>
    /// <returns>the orderId assigned by TDAmeritrade to this order.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<long> PlaceOcoOrderAsync(OcoOrder order, string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders";
        var json = JsonSerializer.Serialize(order, _jsonOptionsWithoutPolymorphicConverters);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponseMessage = await _httpClient.Throttle().PostAsync(path, content);
        switch (httpResponseMessage.StatusCode)
        {
            case HttpStatusCode.Created:
                var location = httpResponseMessage.Headers.First(x => x.Key == "Location");
                var value = location.Value.First();
                var lastSlashIndex = value.LastIndexOf("/", StringComparison.Ordinal);
                var orderIdStr = value.Substring(lastSlashIndex + 1, value.Length - lastSlashIndex - 1);
                long.TryParse(orderIdStr, out var orderId);
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
    public async Task<long> ReplaceOrderAsync(TDOrder replacementOrder, string accountId, long orderId)
    {
        var json = SerializeOrder(replacementOrder);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/orders/{orderId}";
        var httpResponseMessage = await _httpClient.Throttle().PutAsync(path, content);
        switch (httpResponseMessage.StatusCode)
        {
            case HttpStatusCode.Created:
                var location = httpResponseMessage.Headers.First(x => x.Key == "Location");
                var value = location.Value.First();
                var lastSlashIndex = value.LastIndexOf("/", StringComparison.Ordinal);
                var replacementOrderIdStr = value.Substring(lastSlashIndex + 1, value.Length - lastSlashIndex - 1);
                long.TryParse(replacementOrderIdStr, out var replacementOrderId);
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
    public async Task CreateSavedOrderAsync(TDOrder order, string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/savedorders";
        var json = SerializeOrder(order);
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

    public async Task<TDOrder> GetSavedOrderAsync(string accountId, long? savedOrderId)
    {
        if (savedOrderId == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/savedorders/{savedOrderId}";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        var result = JsonSerializer.Deserialize<TDOrder>(json, JsonOptions);
        return result ?? throw new TDAmeritradeSharpException();
    }

    public async Task<IEnumerable<TDOrder>> GetSavedOrdersByPathAsync(string accountId)
    {
        var path = $"https://api.tdameritrade.com/v1/accounts/{accountId}/savedorders";
        var json = await SendRequestAsync(path).ConfigureAwait(false);
        try
        {
            var result = JsonSerializer.Deserialize<IEnumerable<TDOrder>>(json, JsonOptions);
            return result ?? throw new TDAmeritradeSharpException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task DeleteSavedOrderAsync(string accountId, long? savedOrderId)
    {
        if (savedOrderId == null)
        {
            throw new TDAmeritradeSharpException();
        }
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
    public async Task ReplaceSavedOrderAsync(TDOrder replacementOrder, string accountId, long? savedOrderId)
    {
        if (savedOrderId == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var json = SerializeOrder(replacementOrder);
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