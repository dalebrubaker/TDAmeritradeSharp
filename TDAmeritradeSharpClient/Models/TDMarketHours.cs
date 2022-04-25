using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class TDMarketHours
{
    public string? Date { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MarketTypes MarketType { get; set; }

    public string? Exchange { get; set; }
    public string? Category { get; set; }
    public string? Product { get; set; }
    public string? ProductName { get; set; }
    public bool IsOpen { get; set; }
    public SessionHours? SessionHours { get; set; }

    public override string ToString()
    {
        return $"{MarketType} {SessionHours}";
    }
}

public class SessionHours
{
    public PreMarket[]? PreMarket { get; set; }
    public RegularMarket[]? RegularMarket { get; set; }
    public PostMarket[]? PostMarket { get; set; }

    public override string ToString()
    {
        return $"RegularMarket.Length={RegularMarket?.Length}";
    }
}

public class PreMarket
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class RegularMarket
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class PostMarket
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public enum MarketTypes
{
    BOND,
    EQUITY,
    ETF,
    FOREX,
    FUTURE,
    FUTURE_OPTION,
    INDEX,
    INDICAT,
    MUTUAL_FUND,
    OPTION,
    UNKNOWN
}