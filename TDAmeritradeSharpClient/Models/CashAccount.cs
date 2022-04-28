using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

public class CashAccount
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountType Type { get; set; }

    public string AccountId { get; set; } = null!;
    public int RoundTrips { get; set; }
    public bool IsDayTrader { get; set; }
    public bool IsClosingOnlyRestricted { get; set; }
    public Position[]? Positions { get; set; }
    public OrderStrategy[]? OrderStrategies { get; set; }
    public InitialBalancesCash? InitialBalances { get; set; }
    public CurrentBalancesCash? CurrentBalances { get; set; }
    public ProjectedBalancesCash? ProjectedBalances { get; set; }
}

public class InitialBalancesCash
{
    public int AccruedInterest { get; set; }
    public int CashAvailableForTrading { get; set; }
    public int CashAvailableForWithdrawal { get; set; }
    public int CashBalance { get; set; }
    public int BondValue { get; set; }
    public int CashReceipts { get; set; }
    public int LiquidationValue { get; set; }
    public int LongOptionMarketValue { get; set; }
    public int LongStockValue { get; set; }
    public int MoneyMarketFund { get; set; }
    public int MutualFundValue { get; set; }
    public int ShortOptionMarketValue { get; set; }
    public int ShortStockValue { get; set; }
    public bool IsInCall { get; set; }
    public int UnsettledCash { get; set; }
    public int CashDebitCallValue { get; set; }
    public int PendingDeposits { get; set; }
    public int AccountValue { get; set; }
}

public class CurrentBalancesCash
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
    public int CashAvailableForTrading { get; set; }
    public int CashAvailableForWithdrawal { get; set; }
    public int CashCall { get; set; }
    public int LongNonMarginableMarketValue { get; set; }
    public int TotalCash { get; set; }
    public int ShortOptionMarketValue { get; set; }
    public int MutualFundValue { get; set; }
    public int BondValue { get; set; }
    public int CashDebitCallValue { get; set; }
    public int UnsettledCash { get; set; }
}

public class ProjectedBalancesCash
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
    public int CashAvailableForTrading { get; set; }
    public int CashAvailableForWithdrawal { get; set; }
    public int CashCall { get; set; }
    public int LongNonMarginableMarketValue { get; set; }
    public int TotalCash { get; set; }
    public int ShortOptionMarketValue { get; set; }
    public int MutualFundValue { get; set; }
    public int BondValue { get; set; }
    public int CashDebitCallValue { get; set; }
    public int UnsettledCash { get; set; }
}

