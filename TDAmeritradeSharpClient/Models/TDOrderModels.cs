// ReSharper disable IdentifierTypo
// ReSharper disable ClassNeverInstantiated.Global
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

// ReSharper disable InconsistentNaming
public class CancelTime
{
    public string date { get; set; } = null!;
    public bool shortFormat { get; set; }
}

public class OrderLeg
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.instruction instruction { get; set; }

    public Instrument instrument { get; set; } = null!;

    public double quantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.orderLegType orderLegType { get; set; }

    public double legId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.positionEffect positionEffect { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.quantityType quantityType { get; set; }

    public override string ToString()
    {
        return $"{instrument.symbol} {instruction} {quantity}";
    }
}

public class EquityOrderInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.assetType assetType => TDOrderEnums.assetType.EQUITY;

    public string symbol { get; set; } = null!;
    public string cusip { get; set; } = null!;
    public string description { get; set; } = null!;
}

public class FixedIncomeOrderInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.assetType assetType => TDOrderEnums.assetType.FIXED_INCOME;

    public string symbol { get; set; } = null!;
    public string cusip { get; set; } = null!;
    public string description { get; set; } = null!;

    public string maturityDate { get; set; } = null!;
    public double variableRate { get; set; }
    public double factor { get; set; }
}

public class MutualFundOrderInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.assetType assetType => TDOrderEnums.assetType.MUTUAL_FUND;

    public string symbol { get; set; } = null!;
    public string cusip { get; set; } = null!;
    public string description { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.typeMutualFund type { get; set; }
}

public class CashEquivalentOrderInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.assetType assetType => TDOrderEnums.assetType.CASH_EQUIVALENT;

    public string symbol { get; set; } = null!;
    public string cusip { get; set; } = null!;
    public string description { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.typeCashEquivalent type { get; set; }
}

public class OptionOrderInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.assetType assetType => TDOrderEnums.assetType.OPTION;

    public string symbol { get; set; } = null!;
    public string cusip { get; set; } = null!;
    public string description { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.typeOption type { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.putCall putCall { get; set; }

    public string underlyingSymbol { get; set; } = null!;
    public double optionMultiplier { get; set; }
    public List<OptionDeliverables> optionDeliverables { get; set; } = null!;
}

public class OptionDeliverables
{
    public string symbol { get; set; } = null!;
    public double deliverableUnits { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.currencyType currencyType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.assetType assetType { get; set; }
}

public class OrderActivity
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.activityType activityType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.activityTypeExecution executionType { get; set; }

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

public class TDOrder
{
      private string? _price;
    private string? _stopPrice;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.orderType orderType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.session session { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.duration duration { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.orderStrategyType orderStrategyType { get; set; }

    public List<OrderLegBase> orderLegCollection { get; set; } = new();

    public List<IOrderBase> childOrderStrategies { get; set; } = new();

    public string? price
    {
        get
        {
            // TDA enforces 2 or 4 digits this during ReplaceOrder
            double.TryParse(_price, out var value);
            if (value == 0)
            {
                return null;
            }
            var formatStr = value < 1 ? "f4" : "f2";
            _price = value.ToString(formatStr, CultureInfo.InvariantCulture);
            return _price;
        }
        set => _price = value;
    }

    public string? stopPrice
    {
        get
        {
            // TDA enforces 2 or 4 digits this during ReplaceOrder
            double.TryParse(_stopPrice, out var value);
            if (value == 0)
            {
                return null;
            }
            var formatStr = value < 1 ? "f4" : "f2";
            _stopPrice = value.ToString(formatStr, CultureInfo.InvariantCulture);
            return _stopPrice;
        }
        set => _stopPrice = value;
    }

    /// <summary>
    ///     Returns json without type names, suitable for sending to TD Ameritrade
    /// </summary>
    /// <returns></returns>
    public string GetJson()
    {
        var json = JsonSerializer.Serialize(this);
        if (orderType == TDOrderEnums.orderType.MARKET)
        {
            // Remove the price field
            var obj = JsonSerializer.Deserialize<dynamic>(json) as JsonObject;
            obj?.Remove("price");
            json = JsonSerializer.Serialize(obj);
        }
        return json;
    }

    /// <summary>
    ///     Return a deep clone of this order
    /// </summary>
    /// <returns></returns>
    public TDOrder CloneDeep()
    {
        var json = JsonSerializer.Serialize(this);
        var clone = JsonSerializer.Deserialize<TDOrder>(json);
        return clone!;
    }
    
    public CancelTime cancelTime { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.complexOrderStrategyType complexOrderStrategyType { get; set; }

    public double quantity { get; set; }
    public double filledQuantity { get; set; }
    public double remainingQuantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.requestedDestination requestedDestination { get; set; }

    public string destinationLinkName { get; set; } = null!;
    public string releaseTime { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.stopPriceLinkBasis stopPriceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.stopPriceLinkType stopPriceLinkType { get; set; }

    public double stopPriceOffset { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.stopType stopType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.priceLinkBasis priceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.priceLinkType priceLinkType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.taxLotMethod taxLotMethod { get; set; }

    public double activationPrice { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.specialInstruction specialInstruction { get; set; }

    public long orderId { get; set; }
    public bool cancelable { get; set; }
    public bool editable { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.status status { get; set; }

    public string enteredTime { get; set; } = null!;
    public string closeTime { get; set; } = null!;
    public string accountId { get; set; } = null!;
    public List<OrderActivity> orderActivityCollection { get; set; } = null!;
    public List<TDOrder> replacingOrderCollection { get; set; } = null!;
    public string statusDescription { get; set; } = null!;
}

public interface IOrderBase
{
    /// <summary>
    ///     Returns json without type names, suitable for sending to TD Ameritrade
    /// </summary>
    /// <returns></returns>
    string GetJson();
}


public class OcoOrder
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.orderStrategyType orderStrategyType => TDOrderEnums.orderStrategyType.OCO;

    public List<IOrderBase> childOrderStrategies { get; set; } = new();

    /// <summary>
    ///     Returns json without type names, suitable for sending to TD Ameritrade
    /// </summary>
    /// <returns></returns>
    public string GetJson()
    {
        var json = JsonSerializer.Serialize(this);
        return json;
    }
}

public class EquityOrder : IOrderBase
{
    private string? _price;

    private string? _stopPrice;

    public EquityOrder()
    {
        var orderLeg = new EquityOrderLeg();
        orderLegCollection.Add(orderLeg);
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.orderType orderType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.session session { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.duration duration { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.orderStrategyType orderStrategyType { get; set; }

    public List<EquityOrderLeg> orderLegCollection { get; set; } = new();

    public List<IOrderBase> childOrderStrategies { get; set; } = new();

    public string? price
    {
        get
        {
            // TDA enforces 2 or 4 digits this during ReplaceOrder
            double.TryParse(_price, out var value);
            if (value == 0)
            {
                return null;
            }
            var formatStr = value < 1 ? "f4" : "f2";
            _price = value.ToString(formatStr, CultureInfo.InvariantCulture);
            return _price;
        }
        set => _price = value;
    }

    public string? stopPrice
    {
        get
        {
            // TDA enforces 2 or 4 digits this during ReplaceOrder
            double.TryParse(_stopPrice, out var value);
            if (value == 0)
            {
                return null;
            }
            var formatStr = value < 1 ? "f4" : "f2";
            _stopPrice = value.ToString(formatStr, CultureInfo.InvariantCulture);
            return _stopPrice;
        }
        set => _stopPrice = value;
    }

    [JsonIgnore]
    public EquityOrderLeg OrderLeg
    {
        get => orderLegCollection[0];
        set => orderLegCollection[0] = value;
    }

    /// <summary>
    ///     Returns json without type names, suitable for sending to TD Ameritrade
    /// </summary>
    /// <returns></returns>
    public string GetJson()
    {
        var json = JsonSerializer.Serialize(this);
        if (orderType == TDOrderEnums.orderType.MARKET)
        {
            // Remove the price field
            var obj = JsonSerializer.Deserialize<dynamic>(json) as JsonObject;
            obj?.Remove("price");
            json = JsonSerializer.Serialize(obj);
        }
        return json;
    }

    /// <summary>
    ///     Return a deep clone of this order
    /// </summary>
    /// <returns></returns>
    public EquityOrder CloneDeep()
    {
        var json = JsonSerializer.Serialize(this);
        var clone = JsonSerializer.Deserialize<EquityOrder>(json);
        return clone!;
    }
}

public class OrderLegBase
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.instruction instruction { get; set; }

    public Instrument instrument { get; set; } = null!;

    public double quantity { get; set; }
}

public class EquityOrderLeg
{
    public EquityOrderLeg()
    {
        instrument = new EquityOrderInstrument();
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.instruction instruction { get; set; }

    public EquityOrderInstrument instrument { get; set; } = null!;

    public double quantity { get; set; }
}

public class Instrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.assetType assetType { get; set; }

    public string cusip { get; set; } = null!;
    public string symbol { get; set; } = null!;
}

public class TDOrderResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.orderType orderType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.session session { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.duration duration { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.orderStrategyType orderStrategyType { get; set; }

    public List<OrderLeg> orderLegCollection { get; set; } = new();

    public double price { get; set; }
    public CancelTime cancelTime { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.complexOrderStrategyType complexOrderStrategyType { get; set; }

    public double quantity { get; set; }
    public double filledQuantity { get; set; }
    public double remainingQuantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.requestedDestination requestedDestination { get; set; }

    public string destinationLinkName { get; set; } = null!;
    public string releaseTime { get; set; } = null!;
    public double stopPrice { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.stopPriceLinkBasis stopPriceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.stopPriceLinkType stopPriceLinkType { get; set; }

    public double stopPriceOffset { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.stopType stopType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.priceLinkBasis priceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.priceLinkType priceLinkType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.taxLotMethod taxLotMethod { get; set; }

    public double activationPrice { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.specialInstruction specialInstruction { get; set; }

    public long orderId { get; set; }
    public bool cancelable { get; set; }
    public bool editable { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.status status { get; set; }

    public string enteredTime { get; set; } = null!;
    public string closeTime { get; set; } = null!;
    public double accountId { get; set; }
    public List<OrderActivity> orderActivityCollection { get; set; } = null!;
    public List<TDOrder> replacingOrderCollection { get; set; } = null!;
    public List<TDOrder> childOrderStrategies { get; set; } = null!;
    public string statusDescription { get; set; } = null!;

    public string tag { get; set; } = null!;

    public long savedOrderId { get; set; }
    public string savedTime { get; set; } = null!;

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