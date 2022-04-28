using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

public class CashAccount : SecuritiesAccount
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override AccountType Type { get; } = AccountType.CASH;

    public InitialBalancesCash InitialBalances { get; set; } = new InitialBalancesCash();
    public CurrentBalancesCash CurrentBalances { get; set; } = new CurrentBalancesCash();
    public ProjectedBalancesCash ProjectedBalances { get; set; } = new ProjectedBalancesCash();
}

public class InitialBalancesCash
{
    public double AccruedInterest { get; set; }
    public double CashAvailableForTrading { get; set; }
    public double CashAvailableForWithdrawal { get; set; }
    public double CashBalance { get; set; }
    public double BondValue { get; set; }
    public double CashReceipts { get; set; }
    public double LiquidationValue { get; set; }
    public double LongOptionMarketValue { get; set; }
    public double LongStockValue { get; set; }
    public double MoneyMarketFund { get; set; }
    public double MutualFundValue { get; set; }
    public double ShortOptionMarketValue { get; set; }
    public double ShortStockValue { get; set; }
    public bool IsInCall { get; set; }
    public double UnsettledCash { get; set; }
    public double CashDebitCallValue { get; set; }
    public double PendingDeposits { get; set; }
    public double AccountValue { get; set; }
}

public class CurrentBalancesCash
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
    public double CashAvailableForTrading { get; set; }
    public double CashAvailableForWithdrawal { get; set; }
    public double CashCall { get; set; }
    public double LongNonMarginableMarketValue { get; set; }
    public double TotalCash { get; set; }
    public double ShortOptionMarketValue { get; set; }
    public double MutualFundValue { get; set; }
    public double BondValue { get; set; }
    public double CashDebitCallValue { get; set; }
    public double UnsettledCash { get; set; }
}

public class ProjectedBalancesCash
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
    public double CashAvailableForTrading { get; set; }
    public double CashAvailableForWithdrawal { get; set; }
    public double CashCall { get; set; }
    public double LongNonMarginableMarketValue { get; set; }
    public double TotalCash { get; set; }
    public double ShortOptionMarketValue { get; set; }
    public double MutualFundValue { get; set; }
    public double BondValue { get; set; }
    public double CashDebitCallValue { get; set; }
    public double UnsettledCash { get; set; }
}

