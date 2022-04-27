
using System.Text.Json.Serialization;

// ReSharper disable ClassNeverInstantiated.Global

namespace TDAmeritradeSharpClient;

// ReSharper disable InconsistentNaming

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

// ReSharper restore InconsistentNaming

[Serializable]
public class TDOptionChainRequest
{
    /// <summary>
    ///     security id
    /// </summary>
    public string? Symbol { get; set; }

    /// <summary>
    ///     The number of strikes to return above and below the at-the-money price.
    /// </summary>
    public int? StrikeCount { get; set; }

    /// <summary>
    ///     Passing a value returns a Strategy Chain
    /// </summary>
    public TDOptionChainStrategy Strategy { get; set; }

    /// <summary>
    ///     Type of contracts to return in the chai
    /// </summary>
    public TDOptionChainTypes? ContractType { get; set; }

    /// <summary>
    ///     Only return expirations after this date
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    ///     Only return expirations before this date
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    ///     Strike interval for spread strategy chains
    /// </summary>
    public double? Interval { get; set; }

    /// <summary>
    ///     Provide a strike price to return options only at that strike price.
    /// </summary>
    public double? Strike { get; set; }

    /// <summary>
    ///     Returns options for the given range
    /// </summary>
    public TDOptionChainRanges Range { get; set; }

    /// <summary>
    ///     Volatility to use in calculations. ANALYTICAL  only.
    /// </summary>
    public double Volatility { get; set; }

    /// <summary>
    ///     Underlying price to use in calculations. ANALYTICAL  only.
    /// </summary>
    public double UnderlyingPrice { get; set; }

    /// <summary>
    ///     Interest rate to use in calculations. ANALYTICAL  only.
    /// </summary>
    public double InterestRate { get; set; }

    /// <summary>
    ///     Days to expiration to use in calculations. Applies only to ANALYTICAL strategy chains
    /// </summary>
    public double DaysToExpiration { get; set; }

    /// <summary>
    ///     Return only options expiring in the specified month
    /// </summary>
    public string? ExpMonth { get; set; }

    /// <summary>
    ///     Include quotes for options in the option chain. Can be TRUE or FALSE. Default is FALSE.
    /// </summary>
    public bool IncludeQuotes { get; set; }

    /// <summary>
    ///     Type of contracts to return
    /// </summary>
    public TDOptionChainOptionTypes OptionType { get; set; }
}

[Serializable]
public class TDOptionChain
{
    public string? Symbol { get; set; }
    public string? Status { get; set; }
    public TDUnderlying? Underlying { get; set; }
    public string? Strategy { get; set; }
    public double Interval { get; set; }
    public bool IsDelayed { get; set; }
    public bool IsIndex { get; set; }
    public double DaysToExpiration { get; set; }
    public double InterestRate { get; set; }
    public double UnderlyingPrice { get; set; }
    public double Volatility { get; set; }

    public int NumberOfContracts { get; set; }

    public TDOptionMap? CallExpDateMap { get; set; }
    public TDOptionMap? PutExpDateMap { get; set; }
}

[Serializable]
public class TDOptionMap
{
    public DateTime Expires { get; set; }

    public List<TDOptionsAtStrike> OptionsAtStrike { get; set; } = new ();
}

[Serializable]
public class TDOptionsAtStrike
{
    public double StrikePrice { get; set; }
    public List<TDOption>? Options { get; set; }
}

[Serializable]
public class TDOption
{
    public string? PutCall { get; set; } 
    public string? Symbol { get; set; }
    public string? Description { get; set; }
    public string? ExchangeName { get; set; }
    public double Bid { get; set; }
    public double Ask { get; set; }
    public double Last { get; set; }
    public double Mark { get; set; }
    public double BidSize { get; set; }
    public double AskSize { get; set; }
    public double LastSize { get; set; }
    public double HighPrice { get; set; }
    public double LowPrice { get; set; }
    public double OpenPrice { get; set; }
    public double ClosePrice { get; set; }
    public double TotalVolume { get; set; }
    public double? TradeDate { get; set; }
    public long TradeTimeInLong { get; set; }
    public long QuoteTimeInLong { get; set; }
    public double NetChange { get; set; }
    public double Volatility { get; set; }
    public double Delta { get; set; }
    public double Gamma { get; set; }
    public double Theta { get; set; }
    public double Vega { get; set; }
    public double Rho { get; set; }
    public double OpenInterest { get; set; }
    public double TimeValue { get; set; }
    public double TheoreticalOptionValue { get; set; }
    public double TheoreticalVolatility { get; set; }
    public double ExpirationDate { get; set; }
    public double DaysToExpiration { get; set; }
    public string? ExpirationType { get; set; }
    public double LastTradingDay { get; set; }

    public double Multiplier { get; set; }
    public string? SettlementType { get; set; }
    public string? DeliverableNote { get; set; }
    public bool? IsIndexOption { get; set; }
    public double PercentChange { get; set; }
    public double MarkChange { get; set; }
    public double MarkPercentChange { get; set; }
    public double IntrinsicValue { get; set; }
    public bool? IsInTheMoney { get; set; }
    public bool? Mini { get; set; }
    public bool? PennyPilot { get; set; }
    public bool? NonStandard { get; set; }

    [JsonIgnore]
    public DateTime ExpirationDateTime => TDHelpers.FromUnixTimeMilliseconds(ExpirationDate);

    [JsonIgnore]
    public DateTime ExpirationDay => DateTime.Now.AddDays(DaysToExpiration).ToRegularTradingEnd();
}

[Serializable]
public class TDUnderlying
{
    public string? Symbol { get; set; }
    public string? Description { get; set; }
    public double Change { get; set; }
    public double PercentChange { get; set; }
    public double Close { get; set; }
    public double QuoteTime { get; set; }
    public double TradeTime { get; set; }
    public double Bid { get; set; }
    public double Ask { get; set; }
    public double Last { get; set; }
    public double Mark { get; set; }
    public double MarkChange { get; set; }
    public double MarkPercentChange { get; set; }
    public double BidSize { get; set; }
    public double AskSize { get; set; }
    public double HighPrice { get; set; }
    public double LowPrice { get; set; }
    public double OpenPrice { get; set; }
    public double TotalVolume { get; set; }
    public string? ExchangeName { get; set; }
    public double FiftyTwoWeekHigh { get; set; }
    public double FiftyTwoWeekLow { get; set; }
    public bool Delayed { get; set; }
}

[Serializable]
public class TDExpirationDate
{
    public string? Date { get; set; }
}

[Serializable]
public class TDOptionDeliverables
{
    public string? Symbol { get; set; }
    public string? AssetType { get; set; }
    public string? DeliverableUnits { get; set; }
    public string? CurrencyType { get; set; }
}