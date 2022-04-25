namespace TDAmeritradeSharpClient;

[Serializable]
public abstract class TDQuoteBase
{
    public string? Symbol { get; set; }
    public string? Description { get; set; }
    public string? ExchangeName { get; set; }
    public string? SecurityStatus { get; set; }

    public override string ToString()
    {
        return Symbol!;
    }
}

[Serializable]
public class MarketQuoteBase : TDQuoteBase
{
    public double BidPrice { get; set; }
    public double BidSize { get; set; }
    public double AskPrice { get; set; }
    public double AskSize { get; set; }
    public double LastPrice { get; set; }
    public double LastSize { get; set; }
    public double OpenPrice { get; set; }
    public double HighPrice { get; set; }
    public double LowPrice { get; set; }
    public double ClosePrice { get; set; }
    public double NetChange { get; set; }
    public double TotalVolume { get; set; }
    public long QuoteTimeInLong { get; set; }
    public long TradeTimeInLong { get; set; }
    public double Mark { get; set; }
    public string? Exchange { get; set; }
    public double Volatility { get; set; }
}

[Serializable]
public class TDEquityQuote : MarketQuoteBase
{
    public string? BidId { get; set; }
    public string? AskId { get; set; }
    public string? LastId { get; set; }
    public bool Marginable { get; set; }
    public bool Shortable { get; set; }
    public int Digits { get; set; }
    public double _52WkHigh { get; set; }
    public double _52WkLow { get; set; }
    public double PeRatio { get; set; }
    public double DivAmount { get; set; }
    public double DivYield { get; set; }
    public string? DivDate { get; set; }
    public double RegularMarketLastPrice { get; set; }
    public double RegularMarketLastSize { get; set; }
    public double RegularMarketNetChange { get; set; }
    public long RegularMarketTradeTimeInLong { get; set; }
}

[Serializable]
public class TDFundQuote : TDQuoteBase
{
    public double ClosePrice { get; set; }
    public double NetChange { get; set; }
    public double TotalVolume { get; set; }
    public long TradeTimeInLong { get; set; }
    public string? Exchange { get; set; }
    public int Digits { get; set; }
    public double _52WkHigh { get; set; }
    public double _52WkLow { get; set; }
    public double NAv { get; set; }
    public double PeRatio { get; set; }
    public double DivAmount { get; set; }
    public double DivYield { get; set; }
    public string? DivDate { get; set; }
}

[Serializable]
public class FutureQuote : TDQuoteBase
{
    public double BidPriceInDouble { get; set; }
    public double AskPriceInDouble { get; set; }
    public double LastPriceInDouble { get; set; }
    public string? BidId { get; set; }
    public string? AskId { get; set; }
    public double HighPriceInDouble { get; set; }
    public double LowPriceInDouble { get; set; }
    public double ClosePriceInDouble { get; set; }
    public string? Exchange { get; set; }
    public string? LastId { get; set; }
    public double OpenPriceInDouble { get; set; }
    public double ChangeInDouble { get; set; }
    public double FuturePercentChange { get; set; }
    public double OpenInterest { get; set; }
    public double Mark { get; set; }
    public double Tick { get; set; }
    public double TickAmount { get; set; }
    public string? Product { get; set; }
    public string? FuturePriceFormat { get; set; }
    public string? FutureTradingHours { get; set; }
    public bool FutureIsTradable { get; set; }
    public double FutureMultiplier { get; set; }
    public bool FutureIsActive { get; set; }
    public double FutureSettlementPrice { get; set; }
    public string? FutureActiveSymbol { get; set; }
    public double FutureExpirationDate { get; set; }
}

[Serializable]
public class FutureOptionsQuote : TDQuoteBase
{
    public double BidPriceInDouble { get; set; }
    public double AskPriceInDouble { get; set; }
    public double LastPriceInDouble { get; set; }
    public double HighPriceInDouble { get; set; }
    public double LowPriceInDouble { get; set; }
    public double ClosePriceInDouble { get; set; }
    public double OpenPriceInDouble { get; set; }
    public double NetChangeInDouble { get; set; }
    public double OpenInterest { get; set; }
    public double Volatility { get; set; }
    public double MoneyIntrinsicValueInDouble { get; set; }
    public double MultiplierInDouble { get; set; }
    public int Digits { get; set; }
    public double StrikePriceInDouble { get; set; }
    public string? ContractType { get; set; }
    public string? Underlying { get; set; }
    public double TimeValueInDouble { get; set; }
    public double DeltaInDouble { get; set; }
    public double GammaInDouble { get; set; }
    public double ThetaInDouble { get; set; }
    public double VegaInDouble { get; set; }
    public double RhoInDouble { get; set; }
    public double Mark { get; set; }
    public double Tick { get; set; }
    public double TickAmount { get; set; }
    public bool FutureIsTradable { get; set; }
    public string? FutureTradingHours { get; set; }
    public double FuturePercentChange { get; set; }
    public bool FutureIsActive { get; set; }
    public double FutureExpirationDate { get; set; }
    public string? ExpirationType { get; set; }
    public string? ExerciseType { get; set; }
    public bool InTheMoney { get; set; }
}

/// <summary>
///     SPY,$SPX.X, QQQ,$NDX.X, IWM,$RUT.X, IYY,$DJI2MN Vol indexes $VIX.X,$VXX.X,$VXN.X,$RVX.X
/// </summary>
[Serializable]
public class TDIndexQuote : TDQuoteBase
{
    public double LastPrice { get; set; }
    public double OpenPrice { get; set; }
    public double HighPrice { get; set; }
    public double LowPrice { get; set; }
    public double ClosePrice { get; set; }
    public double NetChange { get; set; }
    public double TotalVolume { get; set; }
    public long TradeTimeInLong { get; set; }
    public string? Exchange { get; set; }
    public int Digits { get; set; }
    public double _52WkHigh { get; set; }
    public double _52WkLow { get; set; }
}

[Serializable]
public class TDForexQuote : TDQuoteBase
{
    public double BidPriceInDouble { get; set; }
    public double AskPriceInDouble { get; set; }
    public double LastPriceInDouble { get; set; }
    public double HighPriceInDouble { get; set; }
    public double LowPriceInDouble { get; set; }
    public double ClosePriceInDouble { get; set; }
    public string? Exchange { get; set; }
    public double OpenPriceInDouble { get; set; }
    public double ChangeInDouble { get; set; }
    public double PercentChange { get; set; }
    public int Digits { get; set; }
    public double Tick { get; set; }
    public double TickAmount { get; set; }
    public string? Product { get; set; }
    public string? TradingHours { get; set; }
    public bool IsTradable { get; set; }
    public string? MarketMaker { get; set; }
    public double _52WkHighInDouble { get; set; }
    public double _52WkLowInDouble { get; set; }
    public double Mark { get; set; }
}

[Serializable]
public class TDOptionQuote : MarketQuoteBase
{
    public int OpenInterest { get; set; }
    public double MoneyIntrinsicValue { get; set; }
    public double Multiplier { get; set; }
    public double StrikePrice { get; set; }
    public string? ContractType { get; set; }
    public string? Underlying { get; set; }
    public double TimeValue { get; set; }
    public string? Deliverables { get; set; }
    public double Delta { get; set; }
    public double Gamma { get; set; }
    public double Theta { get; set; }
    public double Vega { get; set; }
    public double Rho { get; set; }
    public double TheoreticalOptionValue { get; set; }
    public double UnderlyingPrice { get; set; }
    public string? UvExpirationType { get; set; }
    public string? SettlementType { get; set; }
}