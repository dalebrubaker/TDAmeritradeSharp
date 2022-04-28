using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

// ReSharper disable InconsistentNaming
public enum AccountType
{
    CASH,
    MARGIN
}
// ReSharper restore InconsistentNaming

public class TDInitialBalances
{
    public double AccruedInterest { get; set; }
    public double AvailableFundsNonMarginableTrade { get; set; }
    public double BondValue { get; set; }
    public double BuyingPower { get; set; }
    public double CashBalance { get; set; }
    public double CashAvailableForTrading { get; set; }
    public double CashReceipts { get; set; }
    public double DayTradingBuyingPower { get; set; }
    public double DayTradingBuyingPowerCall { get; set; }
    public double DayTradingEquityCall { get; set; }
    public double Equity { get; set; }
    public double EquityPercentage { get; set; }
    public double LiquidationValue { get; set; }
    public double LongMarginValue { get; set; }
    public double LongOptionMarketValue { get; set; }
    public double LongStockValue { get; set; }
    public double MaintenanceCall { get; set; }
    public double MaintenanceRequirement { get; set; }
    public double Margin { get; set; }
    public double MarginEquity { get; set; }
    public double MoneyMarketFund { get; set; }
    public double MutualFundValue { get; set; }
    public double RegTCall { get; set; }
    public double ShortMarginValue { get; set; }
    public double ShortOptionMarketValue { get; set; }
    public double ShortStockValue { get; set; }
    public double TotalCash { get; set; }
    public bool IsInCall { get; set; }
    public double PendingDeposits { get; set; }
    public double MarginBalance { get; set; }
    public double ShortBalance { get; set; }
    public double AccountValue { get; set; }
}

public class TDCurrentBalances
{
    public double AccruedInterest { get; set; }
    public double CashBalance { get; set; }
    public double CashReceipts { get; set; }
    public double LongOptionMarketValue { get; set; }
    public double LiquidationValue { get; set; }
    public double LongMarketValue { get; set; }
    public double MoneyMarketFund { get; set; }
    public double Savings { get; set; }
    public double ShortMarketValue { get; set; }
    public double PendingDeposits { get; set; }
    public double AvailableFunds { get; set; }
    public double AvailableFundsNonMarginableTrade { get; set; }
    public double BuyingPower { get; set; }
    public double BuyingPowerNonMarginableTrade { get; set; }
    public double DayTradingBuyingPower { get; set; }
    public double Equity { get; set; }
    public double EquityPercentage { get; set; }
    public double LongMarginValue { get; set; }
    public double MaintenanceCall { get; set; }
    public double MaintenanceRequirement { get; set; }
    public double MarginBalance { get; set; }
    public double RegTCall { get; set; }
    public double ShortBalance { get; set; }
    public double ShortMarginValue { get; set; }
    public double ShortOptionMarketValue { get; set; }
    public double Sma { get; set; }
    public double MutualFundValue { get; set; }
    public double BondValue { get; set; }
}

public class TDProjectedBalances
{
    public double AvailableFunds { get; set; }
    public double AvailableFundsNonMarginableTrade { get; set; }
    public double BuyingPower { get; set; }
    public double DayTradingBuyingPower { get; set; }
    public double DayTradingBuyingPowerCall { get; set; }
    public double MaintenanceCall { get; set; }
    public double RegTCall { get; set; }
    public bool IsInCall { get; set; }
    public double StockBuyingPower { get; set; }
}

public class SecuritiesAccount
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountType Type { get; set; }
    public string AccountId { get; set; } = null!; // string in Account but integer in Order
    public int RoundTrips { get; set; }
    public bool IsDayTrader { get; set; }
    public bool IsClosingOnlyRestricted { get; set; }
    public TDInitialBalances InitialBalances { get; set; } = null!;
    public TDCurrentBalances CurrentBalances { get; set; } = null!;
    public TDProjectedBalances ProjectedBalances { get; set; } = null!;
    
    public override string ToString()
    {
        return $"AccountId={AccountId}";
    }
}

public class TDAccountModel
{
    public SecuritiesAccount SecuritiesAccount { get; set; } = null!;

    public override string ToString()
    {
        return $"AccountId={SecuritiesAccount.AccountId}";
    }
}