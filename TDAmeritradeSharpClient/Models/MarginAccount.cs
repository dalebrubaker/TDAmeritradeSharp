using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

public class MarginAccount
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountType Type { get; set; }

    public string AccountId { get; set; } = null!;
    public int RoundTrips { get; set; }
    public bool IsDayTrader { get; set; }
    public bool IsClosingOnlyRestricted { get; set; }
    public Position[]? Positions { get; set; }
    public OrderStrategy[]? OrderStrategies { get; set; }
    public InitialBalancesMargin? InitialBalances { get; set; }
    public CurrentBalancesMargin? CurrentBalances { get; set; }
    public ProjectedBalancesMargin? ProjectedBalances { get; set; }
}

public class InitialBalancesMargin
{
    public int AccruedInterest { get; set; }
    public int AvailableFundsNonMarginableTrade { get; set; }
    public int BondValue { get; set; }
    public int BuyingPower { get; set; }
    public int CashBalance { get; set; }
    public int CashAvailableForTrading { get; set; }
    public int CashReceipts { get; set; }
    public int DayTradingBuyingPower { get; set; }
    public int DayTradingBuyingPowerCall { get; set; }
    public int DayTradingEquityCall { get; set; }
    public int Equity { get; set; }
    public int EquityPercentage { get; set; }
    public int LiquidationValue { get; set; }
    public int LongMarginValue { get; set; }
    public int LongOptionMarketValue { get; set; }
    public int LongStockValue { get; set; }
    public int MaintenanceCall { get; set; }
    public int MaintenanceRequirement { get; set; }
    public int Margin { get; set; }
    public int MarginEquity { get; set; }
    public int MoneyMarketFund { get; set; }
    public int MutualFundValue { get; set; }
    public int RegTCall { get; set; }
    public int ShortMarginValue { get; set; }
    public int ShortOptionMarketValue { get; set; }
    public int ShortStockValue { get; set; }
    public int TotalCash { get; set; }
    public bool IsInCall { get; set; }
    public int UnsettledCash { get; set; }
    public int PendingDeposits { get; set; }
    public int MarginBalance { get; set; }
    public int ShortBalance { get; set; }
    public int AccountValue { get; set; }
}

public class CurrentBalancesMargin
{
    public int AccruedInterest { get; set; }
    public int CashBalance { get; set; }
    public int CashReceipts { get; set; }
    public int LongOptionMarketValue { get; set; }
    public int LiquidationValue { get; set; }
    public int LongMarketValue { get; set; }
    public int MoneyMarketFund { get; set; }
    public int Savings { get; set; }
    public int ShortMarketValue { get; set; }
    public int PendingDeposits { get; set; }
    public int AvailableFunds { get; set; }
    public int AvailableFundsNonMarginableTrade { get; set; }
    public int BuyingPower { get; set; }
    public int BuyingPowerNonMarginableTrade { get; set; }
    public int DayTradingBuyingPower { get; set; }
    public int DayTradingBuyingPowerCall { get; set; }
    public int Equity { get; set; }
    public int EquityPercentage { get; set; }
    public int LongMarginValue { get; set; }
    public int MaintenanceCall { get; set; }
    public int MaintenanceRequirement { get; set; }
    public int MarginBalance { get; set; }
    public int RegTCall { get; set; }
    public int ShortBalance { get; set; }
    public int ShortMarginValue { get; set; }
    public int ShortOptionMarketValue { get; set; }
    public int Sma { get; set; }
    public int MutualFundValue { get; set; }
    public int BondValue { get; set; }
    public bool IsInCall { get; set; }
    public int StockBuyingPower { get; set; }
    public int OptionBuyingPower { get; set; }
}

public class ProjectedBalancesMargin
{
    public int AccruedInterest { get; set; }
    public int CashBalance { get; set; }
    public int CashReceipts { get; set; }
    public int LongOptionMarketValue { get; set; }
    public int LiquidationValue { get; set; }
    public int LongMarketValue { get; set; }
    public int MoneyMarketFund { get; set; }
    public int Savings { get; set; }
    public int ShortMarketValue { get; set; }
    public int PendingDeposits { get; set; }
    public int AvailableFunds { get; set; }
    public int AvailableFundsNonMarginableTrade { get; set; }
    public int BuyingPower { get; set; }
    public int BuyingPowerNonMarginableTrade { get; set; }
    public int DayTradingBuyingPower { get; set; }
    public int DayTradingBuyingPowerCall { get; set; }
    public int Equity { get; set; }
    public int EquityPercentage { get; set; }
    public int LongMarginValue { get; set; }
    public int MaintenanceCall { get; set; }
    public int MaintenanceRequirement { get; set; }
    public int MarginBalance { get; set; }
    public int RegTCall { get; set; }
    public int ShortBalance { get; set; }
    public int ShortMarginValue { get; set; }
    public int ShortOptionMarketValue { get; set; }
    public int Sma { get; set; }
    public int MutualFundValue { get; set; }
    public int BondValue { get; set; }
    public bool IsInCall { get; set; }
    public int StockBuyingPower { get; set; }
    public int OptionBuyingPower { get; set; }
}