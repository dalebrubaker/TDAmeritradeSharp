namespace TDAmeritradeSharpClient;

// ReSharper disable InconsistentNaming
public class CancelTime
{
    public string date { get; set; } = null!;
    public bool shortFormat { get; set; }
}

public class OrderLegCollection
{
    public TDOrderModelsEnums.orderLegType orderLegType { get; set; }
    public int legId { get; set; }
    public string instrument { get; set; } = null!;
    public TDOrderModelsEnums.instruction instruction { get; set; }
    public TDOrderModelsEnums.positionEffect positionEffect { get; set; }
    public int quantity { get; set; }
    public TDOrderModelsEnums.quantityType quantityType { get; set; }
}

public class OrderInstrumentBase
{
    public TDOrderModelsEnums.assetType assetType { get; set; }
    public string cusip { get; set; } = null!;
    public string symbol { get; set; } = null!;
    public string description { get; set; } = null!;
}

public class EquityOrderInstrument : OrderInstrumentBase
{
}

public class FixedIncomeOrderInstrument : OrderInstrumentBase
{
    public string maturityDate { get; set; } = null!;
    public double variableRate { get; set; }
    public double factor { get; set; }
}

public class MutualFundOrderInstrument : OrderInstrumentBase
{
    public TDOrderModelsEnums.typeMutualFund type { get; set; }
}

public class CashEquivalentOrderInstrument : OrderInstrumentBase
{
    public TDOrderModelsEnums.typeCashEquivalent type { get; set; }
}

public class OptionOrderInstrument : OrderInstrumentBase
{
    public TDOrderModelsEnums.typeOption type { get; set; }
    public TDOrderModelsEnums.putCall putCall { get; set; }
    public string underlyingSymbol { get; set; } = null!;
    public double optionMultiplier { get; set; }
    public List<OptionDeliverables> optionDeliverables { get; set; } = null!;
}

public class OptionDeliverables
{
    public string symbol { get; set; } = null!;
    public double deliverableUnits { get; set; }
    public TDOrderModelsEnums.currencyType currencyType { get; set; }
    public TDOrderModelsEnums.assetType assetType { get; set; }
}

public class OrderActivity
{
    public TDOrderModelsEnums.activityType activityType { get; set; }
    public TDOrderModelsEnums.executionType executionType { get; set; }
    public double quantity { get; set; }
    public double orderRemainingQuantity { get; set; }
    public List<ExecutionLeg> executionLegs { get; set; } = null!;
}

public class ExecutionLeg
{
    public double legId { get; set; }
    public double quantity { get; set; }
    public double mismarkedQuantity { get; set; }
    public double price { get; set; }
    public string time { get; set; } = null!;
}

public class Root
{
    public TDOrderModelsEnums.session session { get; set; }
    public TDOrderModelsEnums.duration duration { get; set; }
    public TDOrderModelsEnums.orderType orderType { get; set; }
    public CancelTime cancelTime { get; set; } = null!;
    public TDOrderModelsEnums.complexOrderStrategyType complexOrderStrategyType { get; set; }
    public int quantity { get; set; }
    public int filledQuantity { get; set; }
    public int remainingQuantity { get; set; }
    public TDOrderModelsEnums.requestedDestination requestedDestination { get; set; }
    public string destinationLinkName { get; set; } = null!;
    public string releaseTime { get; set; } = null!;
    public int stopPrice { get; set; }
    public TDOrderModelsEnums.stopPriceLinkBasis stopPriceLinkBasis { get; set; }
    public TDOrderModelsEnums.stopPriceLinkType stopPriceLinkType { get; set; }
    public int stopPriceOffset { get; set; }
    public TDOrderModelsEnums.stopType stopType { get; set; }
    public TDOrderModelsEnums.priceLinkBasis priceLinkBasis { get; set; }
    public TDOrderModelsEnums.priceLinkType priceLinkType { get; set; }
    public int price { get; set; }
    public TDOrderModelsEnums.taxLotMethod taxLotMethod { get; set; }
    public List<OrderLegCollection> orderLegCollection { get; set; } = null!;
    public int activationPrice { get; set; }
    public TDOrderModelsEnums.specialInstruction specialInstruction { get; set; }
    public TDOrderModelsEnums.orderStrategyType orderStrategyType { get; set; }
    public int orderId { get; set; }
    public bool cancelable { get; set; }
    public bool editable { get; set; }
    public TDOrderModelsEnums.status status { get; set; }
    public string enteredTime { get; set; } = null!;
    public string closeTime { get; set; } = null!;
    public int accountId { get; set; }
    public List<OrderActivity> orderActivityCollection { get; set; } = null!;
    public List<OrderActivity> replacingOrderCollection { get; set; } = null!;
    public List<OrderActivity> childOrderStrategies { get; set; } = null!;
    public string statusDescription { get; set; } = null!;
}