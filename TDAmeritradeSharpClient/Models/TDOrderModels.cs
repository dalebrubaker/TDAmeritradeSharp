// ReSharper disable IdentifierTypo
// ReSharper disable ClassNeverInstantiated.Global
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace TDAmeritradeSharpClient;

// ReSharper disable InconsistentNaming
public class CancelTime
{
    public string date { get; set; } = null!;
    public bool shortFormat { get; set; }
}

public class OrderLeg
{
    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.instruction instruction { get; set; }

    public Instrument instrument { get; set; } = null!;

    public double quantity { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.orderLegType orderLegType { get; set; }

    public double legId { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.positionEffect positionEffect { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.quantityType quantityType { get; set; }

    public override string ToString()
    {
        return $"{instrument.symbol} {instruction} {quantity}";
    }
}

public class OrderInstrumentBase
{
    public string symbol { get; set; } = null!;
    public string cusip { get; set; } = null!;
    public string description { get; set; } = null!;
}

public class EquityOrderInstrument : OrderInstrumentBase
{
    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.assetType assetType => TDOrderModelsEnums.assetType.EQUITY;
}

public class FixedIncomeOrderInstrument : OrderInstrumentBase
{
    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.assetType assetType => TDOrderModelsEnums.assetType.FIXED_INCOME;

    public string maturityDate { get; set; } = null!;
    public double variableRate { get; set; }
    public double factor { get; set; }
}

public class MutualFundOrderInstrument : OrderInstrumentBase
{
    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.assetType assetType => TDOrderModelsEnums.assetType.MUTUAL_FUND;

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.typeMutualFund type { get; set; }
}

public class CashEquivalentOrderInstrument : OrderInstrumentBase
{
    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.assetType assetType => TDOrderModelsEnums.assetType.CASH_EQUIVALENT;

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.typeCashEquivalent type { get; set; }
}

public class OptionOrderInstrument : OrderInstrumentBase
{
    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.assetType assetType => TDOrderModelsEnums.assetType.OPTION;

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.typeOption type { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.putCall putCall { get; set; }

    public string underlyingSymbol { get; set; } = null!;
    public double optionMultiplier { get; set; }
    public List<OptionDeliverables> optionDeliverables { get; set; } = null!;
}

public class OptionDeliverables
{
    public string symbol { get; set; } = null!;
    public double deliverableUnits { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.currencyType currencyType { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.assetType assetType { get; set; }
}

public class OrderActivity
{
    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.activityType activityType { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.activityTypeExecution executionType { get; set; }

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

public class TDOrder : OrderBase
{
    public CancelTime cancelTime { get; set; } = null!;

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.complexOrderStrategyType complexOrderStrategyType { get; set; }

    public double quantity { get; set; }
    public double filledQuantity { get; set; }
    public double remainingQuantity { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.requestedDestination requestedDestination { get; set; }

    public string destinationLinkName { get; set; } = null!;
    public string releaseTime { get; set; } = null!;
    public double stopPrice { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.stopPriceLinkBasis stopPriceLinkBasis { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.stopPriceLinkType stopPriceLinkType { get; set; }

    public double stopPriceOffset { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.stopType stopType { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.priceLinkBasis priceLinkBasis { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.priceLinkType priceLinkType { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.taxLotMethod taxLotMethod { get; set; }

    public double activationPrice { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.specialInstruction specialInstruction { get; set; }

    public string orderId { get; set; } = null!;
    public bool cancelable { get; set; }
    public bool editable { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.status status { get; set; }

    public string enteredTime { get; set; } = null!;
    public string closeTime { get; set; } = null!;
    public string accountId { get; set; } = null!;
    public List<OrderActivity> orderActivityCollection { get; set; } = null!;
    public List<TDOrder> replacingOrderCollection { get; set; } = null!;
    public List<TDOrder> childOrderStrategies { get; set; } = null!;
    public string statusDescription { get; set; } = null!;
}

public class OrderBase
{
    private double _priceNumeric;

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.orderType orderType { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.session session { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.duration duration { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.orderStrategyType orderStrategyType { get; set; }

    public List<OrderLegBase> orderLegCollection { get; set; } = new();

    public string? price { get; set; }

    [JsonIgnore]
    public double priceNumeric
    {
        get => _priceNumeric;
        set
        {
            // TDA enforces 2 or 4 digits this during ReplaceOrder
            _priceNumeric = Math.Round(value, value < 1 ? 4 : 2);
            price = _priceNumeric.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    ///     Returns json without type names, suitable for sending to TD Ameritrade
    /// </summary>
    /// <returns></returns>
    public string GetJson()
    {
        var json = JsonConvert.SerializeObject(this);
        if (orderType == TDOrderModelsEnums.orderType.MARKET)
        {
            // Remove the price field
            var obj = JsonConvert.DeserializeObject<dynamic>(json) as JObject;
            obj?.Remove("price");
            json = JsonConvert.SerializeObject(obj);
        }
        return json;
    }

    /// <summary>
    ///     Return a deep clone of this order
    /// </summary>
    /// <returns></returns>
    public OrderBase CloneDeep()
    {
        // Must use settings in order to deserialize with proper derived classes
        var settingsDefault = new JsonSerializerSettings();
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };
        var json = JsonConvert.SerializeObject(this, settings);
        var clone = JsonConvert.DeserializeObject<OrderBase>(json, settings);
        return clone;
    }
}

public class EquityOrder : OrderBase
{
    public EquityOrder()
    {
        var orderLeg = new EquityOrderLeg();
        orderLegCollection.Add(orderLeg);
    }

    [JsonIgnore]
    public OrderLegBase OrderLeg
    {
        get => orderLegCollection[0];
        set => orderLegCollection[0] = value;
    }
}

public class OrderLegBase
{
    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.instruction instruction { get; set; }

    public OrderInstrumentBase instrument { get; set; } = null!;

    public double quantity { get; set; }
}

public class EquityOrderLeg : OrderLegBase
{
    public EquityOrderLeg()
    {
        instrument = new EquityOrderInstrument();
    }
}

public class Instrument
{
    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.assetType assetType { get; set; }

    public string cusip { get; set; } = null!;
    public string symbol { get; set; } = null!;
}

public class TDOrderResponse
{
    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.orderType orderType { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.session session { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.duration duration { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.orderStrategyType orderStrategyType { get; set; }

    public List<OrderLeg> orderLegCollection { get; set; } = new();

    public double price { get; set; }
    public CancelTime cancelTime { get; set; } = null!;

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.complexOrderStrategyType complexOrderStrategyType { get; set; }

    public double quantity { get; set; }
    public double filledQuantity { get; set; }
    public double remainingQuantity { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.requestedDestination requestedDestination { get; set; }

    public string destinationLinkName { get; set; } = null!;
    public string releaseTime { get; set; } = null!;
    public double stopPrice { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.stopPriceLinkBasis stopPriceLinkBasis { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.stopPriceLinkType stopPriceLinkType { get; set; }

    public double stopPriceOffset { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.stopType stopType { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.priceLinkBasis priceLinkBasis { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.priceLinkType priceLinkType { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.taxLotMethod taxLotMethod { get; set; }

    public double activationPrice { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.specialInstruction specialInstruction { get; set; }

    public string orderId { get; set; } = null!;
    public bool cancelable { get; set; }
    public bool editable { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TDOrderModelsEnums.status status { get; set; }

    public string enteredTime { get; set; } = null!;
    public string closeTime { get; set; } = null!;
    public string accountId { get; set; } = null!;
    public List<OrderActivity> orderActivityCollection { get; set; } = null!;
    public List<TDOrder> replacingOrderCollection { get; set; } = null!;
    public List<TDOrder> childOrderStrategies { get; set; } = null!;
    public string statusDescription { get; set; } = null!;

    public string tag { get; set; } = null!;

    public override string ToString()
    {
        var result = $"accountId={accountId} orderId={orderId} {orderLegCollection[0]} {orderType}";
        if (price != 0)
        {
            result += $" {price}";
        }
        return result;
    }
}