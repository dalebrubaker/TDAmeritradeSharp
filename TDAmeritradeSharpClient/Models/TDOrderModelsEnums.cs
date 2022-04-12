// ReSharper disable InconsistentNaming
namespace TDAmeritradeSharpClient;

public class TDOrderModelsEnums
{
    [Serializable]
    public enum activityType
    {
        EXECUTION,
        ORDER_ACTION
    }

    [Serializable]
    public enum complexOrderStrategyType
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
    public enum duration
    {
        DAY,
        GOOD_TILL_CANCEL,
        FILL_OR_KILL
    }

    [Serializable]
    public enum orderType
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
    public enum requestedDestination
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
    public enum session
    {
        NORMAL,
        AM,
        PM,
        SEAMLESS
    }

    [Serializable]
    public enum stopPriceLinkBasis
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
    public enum stopPriceLinkType
    {
        VALUE,
        PERCENT,
        TICK
    }

    [Serializable]
    public enum positionEffect
    {
        OPENING,
        CLOSING,
        AUTOMATIC
    }
    
    [Serializable]
    public enum priceLinkType
    {
        VALUE,
        PERCENT,
        TICK
    }

    [Serializable]
    public enum stopType
    {
        STANDARD,
        BID,
        ASK,
        LAST,
        MARK,
    }
    
    [Serializable]
    public enum priceLinkBasis
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
    public enum taxLotMethod
    {
        FIFO,
        LIFO,
        HIGH_COST,
        LOW_COST,
        AVERAGE_COST,
        SPECIFIC_LOT,
    }
    
    [Serializable]
    public enum orderLegType
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
    public enum assetType
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
    public enum instruction
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
    public enum quantityType
    {
        ALL_SHARES,
        DOLLARS,
        SHARES,
    }
    
    [Serializable]
    public enum specialInstruction
    {
        ALL_OR_NONE,
        DO_NOT_REDUCE,
        ALL_OR_NONE_DO_NOT_REDUCE,
    }
    
    [Serializable]
    public enum orderStrategyType
    {
        SINGLE,
        OCO,
        TRIGGER,
    }
    
    [Serializable]
    public enum status
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
    public enum typeMutualFund
    {
        NOT_APPLICABLE,
        OPEN_END_NON_TAXABLE,
        OPEN_END_TAXABLE,
        NO_LOAD_NON_TAXABLE,
        NO_LOAD_TAXABLE,
    }
    
    [Serializable]
    public enum typeCashEquivalent
    {
        SAVINGS,
        MONEY_MARKET_FUND,
    }
    
    [Serializable]
    public enum typeOption
    {
        VANILLA,
        BINARY,
        BARRIER,
    }
    
    [Serializable]
    public enum putCall
    {
        PUT,
        CALL,
    }
    
    [Serializable]
    public enum currencyType
    {
        USD,
        CAD,
        EUR,
        JPY,
    }
    
    [Serializable]
    public enum executionType
    {
        EXECUTION,
        ORDER_ACTION,
    }
    
    [Serializable]
    public enum activityTypeExecution
    {
        FILL,
    }

}