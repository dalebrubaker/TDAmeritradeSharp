﻿// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

/// <summary>
///     common interface for all realtime stream signals
/// </summary>
public interface ISignal
{
    /// <summary>
    ///     UNIX
    /// </summary>
    public double Timestamp { get; set; }

    /// <summary>
    ///     Ticker symbol in upper case.
    /// </summary>
    public string Symbol { get; set; }

    public DateTime TimeStamp { get; }
}

[Serializable]
public struct TDHeartbeatSignal
{
    /// <summary>
    ///     UNIX
    /// </summary>
    public double Timestamp { get; set; }

    public DateTime TimeStamp => TDHelpers.FromUnixTimeMilliseconds(Timestamp);
}

// ReSharper disable InconsistentNaming

[Serializable]
public enum TDBookOptions
{
    LISTED_BOOK,
    NASDAQ_BOOK,
    OPTIONS_BOOK,
    FUTURES_BOOK,
    FOREX_BOOK,
    FUTURES_OPTIONS_BOOK
}
// ReSharper restore InconsistentNaming

[Serializable]
public struct TDBookSignal : ISignal
{
    /// <summary>
    ///     UNIX
    /// </summary>
    public double Timestamp { get; set; }

    /// <summary>
    ///     0 Ticker symbol in upper case.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    ///     Book source
    /// </summary>
    public TDBookOptions _id;

    /// <summary>
    ///     2 bids
    /// </summary>
    public TDBookLevel[] _bids;

    /// <summary>
    ///     3 asks
    /// </summary>
    public TDBookLevel[] _asks;

    public DateTime TimeStamp => TDHelpers.FromUnixTimeMilliseconds(Timestamp);
}

[Serializable]
public struct TDBookLevel
{
    /// <summary>
    ///     0 this price level
    /// </summary>
    [JsonPropertyName("0")]
    public double _price;

    /// <summary>
    ///     2 total volume at this level
    /// </summary>
    [JsonPropertyName("1")]
    public double _quantity;
}

[Serializable]
public struct TDQuoteSignal : ISignal
{
    /// <summary>
    ///     UNIX
    /// </summary>
    public double Timestamp { get; set; }

    /// <summary>
    ///     0 Ticker symbol in upper case.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    ///     1 Current Best Bid Price
    /// </summary>
    public double _bidprice;

    /// <summary>
    ///     2 Current Best Ask Price
    /// </summary>
    public double _askprice;

    /// <summary>
    ///     4 Number of shares for bid
    /// </summary>
    public double _bidsize;

    /// <summary>
    ///     5 Number of shares for ask
    /// </summary>
    public double _asksize;

    /// <summary>
    ///     3 Price at which the last trade was matched
    /// </summary>
    public double _lastprice;

    /// <summary>
    ///     9 Number of shares traded with last trade
    /// </summary>
    public double _lastsize;

    /// <summary>
    ///     8 Aggregated shares traded throughout the day, including pre/post market hours.
    /// </summary>
    public long _totalvolume;

    /// <summary>
    ///     28 Previous day’s opening price
    /// </summary>
    public double _openprice;

    /// <summary>
    ///     15 Previous day’s closing price
    /// </summary>
    public double _closeprice;

    /// <summary>
    ///     13 Day’s low trade price
    /// </summary>
    public double _lowprice;

    /// <summary>
    ///     12 Day’s high trade price
    /// </summary>
    public double _highprice;

    /// <summary>
    ///     10 Trade time of the last trade
    /// </summary>
    public double _tradetime;

    /// <summary>
    ///     11 Quote time of the last trade
    /// </summary>
    public double _quotetime;

    /// <summary>
    ///     7 Exchange with the best bid
    /// </summary>
    public char _bidid;

    /// <summary>
    ///     6 Exchange with the best ask
    /// </summary>
    public char _askid;

    /// <summary>
    ///     14 Indicates Up or Downtick(NASDAQ NMS & Small Cap)
    /// </summary>
    public char _bidtick;

    /// <summary>
    ///     24 Option Risk/Volatility Measurement
    /// </summary>
    public double _volatility;

    public DateTime TimeStamp => TDHelpers.FromUnixTimeMilliseconds(Timestamp);
    public DateTime QuoteTime => TDHelpers.FromUnixTimeMilliseconds(_quotetime);
    public DateTime TradeTime => TDHelpers.FromUnixTimeMilliseconds(_tradetime);
}

[Serializable]
public struct TDTimeSaleSignal : ISignal
{
    /// <summary>
    ///     UNIX
    /// </summary>
    public double Timestamp { get; set; }

    /// <summary>
    ///     0 Ticker symbol in upper case.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    ///     order
    /// </summary>
    public long _sequence;

    /// <summary>
    ///     1 Trade time of the last trade
    /// </summary>
    public double _tradetime;

    /// <summary>
    ///     2 Price at which the last trade was matched
    /// </summary>
    public double _lastprice;

    /// <summary>
    ///     3 Number of shares traded with last trade
    /// </summary>
    public double _lastsize;

    /// <summary>
    ///     4 Number of Number of shares for bid
    /// </summary>
    public long _lastsequence;

    public DateTime TimeStamp => TDHelpers.FromUnixTimeMilliseconds(Timestamp);
    public DateTime TradeTime => TDHelpers.FromUnixTimeMilliseconds(_tradetime);
}

[Serializable]
public struct TDChartSignal : ISignal
{
    /// <summary>
    ///     UNIX
    /// </summary>
    public double Timestamp { get; set; }

    /// <summary>
    ///     0 Ticker symbol in upper case.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    ///     1 Opening price for the minute
    /// </summary>
    public double _openprice;

    /// <summary>
    ///     2 Highest price for the minute
    /// </summary>
    public double _highprice;

    /// <summary>
    ///     3 Chart’s lowest price for the minute
    /// </summary>
    public double _lowprice;

    /// <summary>
    ///     4 Closing price for the minute
    /// </summary>
    public double _closeprice;

    /// <summary>
    ///     5 Total volume for the minute
    /// </summary>
    public double _volume;

    /// <summary>
    ///     6 Identifies the candle minute
    /// </summary>
    public long _sequence;

    /// <summary>
    ///     7 Milliseconds since Epoch
    /// </summary>
    public long _charttime;

    /// <summary>
    ///     8 Not useful
    /// </summary>
    public int _chartday;

    public DateTime TimeStamp => TDHelpers.FromUnixTimeMilliseconds(Timestamp);

    public DateTime ChartTime => TDHelpers.FromUnixTimeMilliseconds(_charttime);

    public int ChartIndex => ChartTime.ToCandleIndex(1);
}

// ReSharper disable InconsistentNaming
[Serializable]
public enum TDChartSubs
{
    CHART_EQUITY,
    CHART_OPTIONS,
    CHART_FUTURES
}

[Serializable]
public enum TDTimeSaleServices
{
    TIMESALE_EQUITY,
    TIMESALE_FOREX,
    TIMESALE_FUTURES,
    TIMESALE_OPTIONS
}

[Serializable]
public enum TDQOSLevels
{
    /// <summary>
    ///     500ms
    /// </summary>
    EXPRESS,

    /// <summary>
    ///     750ms
    /// </summary>
    REALTIME,

    /// <summary>
    ///     1000ms
    /// </summary>
    FAST,

    /// <summary>
    ///     1500ms
    /// </summary>
    MODERATE,

    /// <summary>
    ///     5000ms
    /// </summary>
    DELAYED,

    /// <summary>
    ///     3000ms
    /// </summary>
    SLOW
}
// ReSharper restore InconsistentNaming

[Serializable]
public class TDRealtimeRequest
{
    public string? Service { get; set; }
    public string? Command { get; set; }
    public int Requestid { get; set; }
    public string? Account { get; set; } // the AccountId
    public string? Source { get; set; }
    public object? Parameters { get; set; }
}

[Serializable]
public class TDRealtimeRequestContainer
{
    public TDRealtimeRequest[]? Requests { get; set; }
}

[Serializable]
public class TDRealtimeResponseContainer
{
    public TDRealtimeResponse[]? Response { get; set; }
}

[Serializable]
public class TDRealtimeResponse
{
    public string? Service { get; set; }
    public string? Requestid { get; set; }
    public string? Command { get; set; }
    public double Timestamp { get; set; }
    public TDRealtimeContent? Content { get; set; }

    public DateTime TimeStamp => TDHelpers.FromUnixTimeMilliseconds(Timestamp);
}

[Serializable]
public class TDRealtimeContent
{
    public int Code { get; set; }
    public string? Msg { get; set; }
}