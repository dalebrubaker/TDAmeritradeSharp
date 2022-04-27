// ReSharper disable InconsistentNaming
namespace TDAmeritradeSharpClient;

public class TDOrderEnums
{
    [Serializable]
    public enum ActivityType
    {
        EXECUTION,
        ORDER_ACTION
    }

    [Serializable]
    public enum ComplexOrderStrategyType
    {
        NONE,
        COVERED,
        VERTICAL,
        BACK_RATIO,
        CALENDAR,
        DIAGONAL,
        STRADDLE,
        STRANGLE,
        COLLAR_SYNTHETIC,
        BUTTERFLY,
        CONDOR,
        IRON_CONDOR,
        VERTICAL_ROLL,
        COLLAR_WITH_STOCK,
        DOUBLE_DIAGONAL,
        UNBALANCED_BUTTERFLY,
        UNBALANCED_CONDOR,
        UNBALANCED_IRON_CONDOR,
        UNBALANCED_VERTICAL_ROLL,
        CUSTOM
    }

    [Serializable]
    public enum Duration
    {
        DAY,
        GOOD_TILL_CANCEL,
        FILL_OR_KILL
    }

    [Serializable]
    public enum OrderType
    {
        MARKET,
        LIMIT,
        STOP,
        STOP_LIMIT,
        TRAILING_STOP,
        MARKET_ON_CLOSE,
        EXERCISE,
        TRAILING_STOP_LIMIT,
        NET_DEBIT,
        NET_CREDIT,
        NET_ZERO
    }

    [Serializable]
    public enum RequestedDestination
    {
        INET,
        ECN_ARCA,
        CBOE,
        AMEX,
        PHLX,
        ISE,
        BOX,
        NYSE,
        NASDAQ,
        BATS,
        C2,
        AUTO
    }

    [Serializable]
    public enum Session
    {
        NORMAL,
        AM,
        PM,
        SEAMLESS
    }

    [Serializable]
    public enum StopPriceLinkBasis
    {
        MANUAL,
        BASE,
        TRIGGER,
        LAST,
        BID,
        ASK,
        ASK_BID,
        MARK,
        AVERAGE
    }

    [Serializable]
    public enum StopPriceLinkType
    {
        VALUE,
        PERCENT,
        TICK
    }

    [Serializable]
    public enum PositionEffect
    {
        OPENING,
        CLOSING,
        AUTOMATIC
    }
    
    [Serializable]
    public enum PriceLinkType
    {
        VALUE,
        PERCENT,
        TICK
    }

    [Serializable]
    public enum StopType
    {
        STANDARD,
        BID,
        ASK,
        LAST,
        MARK,
    }
    
    [Serializable]
    public enum PriceLinkBasis
    {
        MANUAL,
        BASE,
        TRIGGER,
        LAST,
        BID,
        ASK,
        ASK_BID,
        MARK,
        AVERAGE
    }
    
    [Serializable]
    public enum TaxLotMethod
    {
        FIFO,
        LIFO,
        HIGH_COST,
        LOW_COST,
        AVERAGE_COST,
        SPECIFIC_LOT,
    }
    
    [Serializable]
    public enum OrderLegType
    {
        EQUITY,
        OPTION,
        INDEX,
        MUTUAL_FUND,
        CASH_EQUIVALENT,
        FIXED_INCOME,
        CURRENCY,
    }
    
    [Serializable]
    public enum AssetType
    {
        EQUITY,
        OPTION,
        INDEX,
        MUTUAL_FUND,
        CASH_EQUIVALENT,
        FIXED_INCOME,
        CURRENCY,
    }
    
    [Serializable]
    public enum Instruction
    {
        BUY,
        SELL,
        BUY_TO_COVER,
        SELL_SHORT,
        BUY_TO_OPEN,
        BUY_TO_CLOSE,
        SELL_TO_OPEN,
        SELL_TO_CLOSE,
        EXCHANGE
    }
    
    [Serializable]
    public enum QuantityType
    {
        ALL_SHARES,
        DOLLARS,
        SHARES,
    }
    
    [Serializable]
    public enum SpecialInstruction
    {
        ALL_OR_NONE,
        DO_NOT_REDUCE,
        ALL_OR_NONE_DO_NOT_REDUCE,
    }
    
    [Serializable]
    public enum OrderStrategyType
    {
        SINGLE,
        OCO,
        TRIGGER,
    }
    
    [Serializable]
    public enum Status
    {
        AWAITING_PARENT_ORDER,
        AWAITING_CONDITION,
        AWAITING_MANUAL_REVIEW,
        ACCEPTED,
        AWAITING_UR_OUT,
        PENDING_ACTIVATION,
        QUEUED,
        WORKING,
        REJECTED,
        PENDING_CANCEL,
        CANCELED,
        PENDING_REPLACE,
        REPLACED,
        FILLED,
        EXPIRED,
    }
    
    [Serializable]
    public enum TypeMutualFund
    {
        NOT_APPLICABLE,
        OPEN_END_NON_TAXABLE,
        OPEN_END_TAXABLE,
        NO_LOAD_NON_TAXABLE,
        NO_LOAD_TAXABLE,
    }
    
    [Serializable]
    public enum TypeCashEquivalent
    {
        SAVINGS,
        MONEY_MARKET_FUND,
    }
    
    [Serializable]
    public enum TypeOption
    {
        VANILLA,
        BINARY,
        BARRIER,
    }
    
    [Serializable]
    public enum PutCall
    {
        PUT = 0,
        CALL = 1,
    }
    
    [Serializable]
    public enum CurrencyType
    {
        USD,
        CAD,
        EUR,
        JPY,
    }
    
    [Serializable]
    public enum ExecutionType
    {
        EXECUTION,
        ORDER_ACTION,
    }
    
    [Serializable]
    public enum ActivityTypeExecution
    {
        FILL,
    }
}