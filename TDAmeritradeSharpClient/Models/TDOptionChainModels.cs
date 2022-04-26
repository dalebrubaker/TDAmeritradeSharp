// ReSharper disable InconsistentNaming

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

// ReSharper disable ClassNeverInstantiated.Global

namespace TDAmeritradeSharpClient;

[Serializable]
public enum TDOptionChainTypes
{
    ALL,
    PUT,
    CALL
}

[Serializable]
public enum TDOptionChainStrategy
{
    SINGLE,
    ANALYTICAL,
    COVERED,
    VERTICAL,
    CALENDAR,
    STRANGLE,
    STRADDLE,
    BUTTERFLY,
    CONDOR,
    DIAGNOL,
    COLLAR,
    ROLL
}

[Serializable]
public enum TDOptionChainOptionTypes
{
    /// <summary>
    ///     All
    /// </summary>
    ALL,

    /// <summary>
    ///     Standard
    /// </summary>
    S,

    /// <summary>
    ///     NonStandard
    /// </summary>
    NS
}

/// <summary>
///     ITM: In-the-money
///     NTM: Near-the-money
///     OTM: Out-of-the-money
///     SAK: Strikes Above Market
///     SBK: Strikes Below Market
///     SNK: Strikes Near Market
///     ALL: All Strikes
/// </summary>
[Serializable]
public enum TDOptionChainRanges
{
    /// ALL: All Strikes
    ALL,

    /// ITM: In-the-money
    ITM,

    /// NTM: Near-the-money
    NTM,

    /// OTM: Out-of-the-money
    OTM,

    /// SAK: Strikes Above Market
    SAK,

    /// SBK: Strikes Below Market
    SBK,

    /// SNK: Strikes Near Market
    SNK
}

[Serializable]
public class TDOptionChainRequest
{
    /// <summary>
    ///     security id
    /// </summary>
    public string? symbol { get; set; }

    /// <summary>
    ///     The number of strikes to return above and below the at-the-money price.
    /// </summary>
    public int? strikeCount { get; set; }

    /// <summary>
    ///     Passing a value returns a Strategy Chain
    /// </summary>
    public TDOptionChainStrategy strategy { get; set; }

    /// <summary>
    ///     Type of contracts to return in the chai
    /// </summary>
    public TDOptionChainTypes? contractType { get; set; }

    /// <summary>
    ///     Only return expirations after this date
    /// </summary>
    public DateTime? fromDate { get; set; }

    /// <summary>
    ///     Only return expirations before this date
    /// </summary>
    public DateTime? toDate { get; set; }

    /// <summary>
    ///     Strike interval for spread strategy chains
    /// </summary>
    public double? interval { get; set; }

    /// <summary>
    ///     Provide a strike price to return options only at that strike price.
    /// </summary>
    public double? strike { get; set; }

    /// <summary>
    ///     Returns options for the given range
    /// </summary>
    public TDOptionChainRanges range { get; set; }

    /// <summary>
    ///     Volatility to use in calculations. ANALYTICAL  only.
    /// </summary>
    public double volatility { get; set; }

    /// <summary>
    ///     Underlying price to use in calculations. ANALYTICAL  only.
    /// </summary>
    public double underlyingPrice { get; set; }

    /// <summary>
    ///     Interest rate to use in calculations. ANALYTICAL  only.
    /// </summary>
    public double interestRate { get; set; }

    /// <summary>
    ///     Days to expiration to use in calculations. Applies only to ANALYTICAL strategy chains
    /// </summary>
    public double daysToExpiration { get; set; }

    /// <summary>
    ///     Return only options expiring in the specified month
    /// </summary>
    public string? expMonth { get; set; }

    /// <summary>
    ///     Include quotes for options in the option chain. Can be TRUE or FALSE. Default is FALSE.
    /// </summary>
    public bool includeQuotes { get; set; }

    /// <summary>
    ///     Type of contracts to return
    /// </summary>
    public TDOptionChainOptionTypes optionType { get; set; }
}

[JsonConverter(typeof(TDOptionChainConverter))]
[Serializable]
public class TDOptionChain
{
    public string? symbol { get; set; }
    public string? status { get; set; }
    public TDUnderlying? underlying { get; set; }
    public string? strategy { get; set; } // TODO Should be enum?
    public double interval { get; set; }
    public bool isDelayed { get; set; }
    public bool isIndex { get; set; }
    public double daysToExpiration { get; set; }
    public double interestRate { get; set; }
    public double underlyingPrice { get; set; }
    public double volatility { get; set; }

    public int numberOfContracts { get; set; }

    public TDOptionMap? callExpDateMap { get; set; }
    public TDOptionMap? putExpDateMap { get; set; }
}

public class TDOptionChainConverter : JsonConverter<TDOptionChain>
{
    public override TDOptionChain? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);
        if (node == null)
        {
            throw new InvalidOperationException();
        }
        var result = new TDOptionChain
        {
            symbol = node["symbol"]?.GetValue<string>(),
            status = node["status"]?.GetValue<string>(),
            underlying = node["underlying"].Deserialize<TDUnderlying>(),
            strategy = node["strategy"]?.GetValue<string>(),
            interval = node["interval"]!.GetValue<double>(),
            isDelayed = node["isDelayed"]!.GetValue<bool>(),
            isIndex = node["isIndex"]!.GetValue<bool>(),
            daysToExpiration = node["daysToExpiration"]!.GetValue<double>(),
            interestRate = node["interestRate"]!.GetValue<double>(),
            underlyingPrice = node["underlyingPrice"]!.GetValue<double>(),
            volatility = node["volatility"]!.GetValue<double>(),
            callExpDateMap = GetMap(node["callExpDateMap"]),
            putExpDateMap = GetMap(node["putExpDateMap"])
        };
        return result;
    }


     public TDOptionMap GetMap(JsonNode? node)
     {
         var exp = new TDOptionMap();
         var jsonObject = (JsonObject)node!;
         foreach (KeyValuePair<string,JsonNode?> pair in jsonObject)
         {
             exp.expires = DateTime.Parse(pair.Key.Split(':')[0]);
             exp.optionsAtStrike = new List<TDOptionsAtStrike>(); // This is an array in TDA json although we seem to have only one instance in each array
             var optionsAtStrike = new TDOptionsAtStrike();
             exp.optionsAtStrike.Add(optionsAtStrike);
             Debug.Assert(jsonObject.Count > 0);
             var strikeObject = (JsonObject)pair.Value!;
             var strikeNode = strikeObject.FirstOrDefault();
             double.TryParse(strikeNode.Key, out var strikePrice);
             optionsAtStrike.strikePrice = strikePrice;
             optionsAtStrike.options = new List<TDOption>();
             var array = strikeNode.Value?.AsArray();
             foreach (var optionNode in array!)
             {
                 var option = optionNode.Deserialize<TDOption>();
                 optionsAtStrike.options.Add(option ?? throw new InvalidOperationException());
             }
         }
         return exp;
     }

    public override void Write(Utf8JsonWriter writer, TDOptionChain value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class TDOptionMap
{
    public DateTime expires { get; set; }

    public List<TDOptionsAtStrike> optionsAtStrike { get; set; } = new ();
}

[Serializable]
public class TDOptionsAtStrike
{
    public double strikePrice { get; set; }
    public List<TDOption>? options { get; set; }
}

[Serializable]
public class TDOption
{
    public string? putCall { get; set; } // TODO should be enum?
    public string? symbol { get; set; }
    public string? description { get; set; }
    public string? exchangeName { get; set; }
    public double bid { get; set; }
    public double ask { get; set; }
    public double last { get; set; }
    public double mark { get; set; }
    public double bidSize { get; set; }
    public double askSize { get; set; }
    public double lastSize { get; set; }
    public double highPrice { get; set; }
    public double lowPrice { get; set; }
    public double openPrice { get; set; }
    public double closePrice { get; set; }
    public double totalVolume { get; set; }
    public double? tradeDate { get; set; }
    public long tradeTimeInLong { get; set; }
    public long quoteTimeInLong { get; set; }
    public double netChange { get; set; }
    public double volatility { get; set; }
    public double delta { get; set; }
    public double gamma { get; set; }
    public double theta { get; set; }
    public double vega { get; set; }
    public double rho { get; set; }
    public double openInterest { get; set; }
    public double timeValue { get; set; }
    public double theoreticalOptionValue { get; set; }
    public double theoreticalVolatility { get; set; }
    public double expirationDate { get; set; }
    public double daysToExpiration { get; set; }
    public string? expirationType { get; set; } // TODO enum?
    public double lastTradingDay { get; set; }

    public double multiplier { get; set; }
    public string? settlementType { get; set; }
    public string? deliverableNote { get; set; }
    public bool? isIndexOption { get; set; }
    public double percentChange { get; set; }
    public double markChange { get; set; }
    public double markPercentChange { get; set; }
    public double intrinsicValue { get; set; }
    public bool? isInTheMoney { get; set; }
    public bool? mini { get; set; }
    public bool? pennyPilot { get; set; }
    public bool? nonStandard { get; set; }

    [JsonIgnore]
    public DateTime ExpirationDate => TDHelpers.FromUnixTimeMilliseconds(expirationDate);

    [JsonIgnore]
    public DateTime ExpirationDay => DateTime.Now.AddDays(daysToExpiration).ToRegularTradingEnd();
}

[Serializable]
public class TDUnderlying
{
    public string? symbol { get; set; }
    public string? description { get; set; }
    public double change { get; set; }
    public double percentChange { get; set; }
    public double close { get; set; }
    public double quoteTime { get; set; }
    public double tradeTime { get; set; }
    public double bid { get; set; }
    public double ask { get; set; }
    public double last { get; set; }
    public double mark { get; set; }
    public double markChange { get; set; }
    public double markPercentChange { get; set; }
    public double bidSize { get; set; }
    public double askSize { get; set; }
    public double highPrice { get; set; }
    public double lowPrice { get; set; }
    public double openPrice { get; set; }
    public double totalVolume { get; set; }
    public string? exchangeName { get; set; }
    public double fiftyTwoWeekHigh { get; set; }
    public double fiftyTwoWeekLow { get; set; }
    public bool delayed { get; set; }
}

[Serializable]
public class TDExpirationDate
{
    public string? date { get; set; }
}

[Serializable]
public class TDOptionDeliverables
{
    public string? symbol { get; set; }
    public string? assetType { get; set; }
    public string? deliverableUnits { get; set; }
    public string? currencyType { get; set; }
}