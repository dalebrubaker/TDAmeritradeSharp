// ReSharper disable IdentifierTypo
// ReSharper disable ClassNeverInstantiated.Global
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
    private double _price;
    private double? _stopPrice;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.orderType orderType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.session session { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.duration duration { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.orderStrategyType orderStrategyType { get; set; }

    public List<OrderLegBase> orderLegCollection { get; set; } = new();

    /// <summary>
    ///     Must be object, not IOrderBase, to get serialization in System.Text.Json
    /// </summary>
    public List<object> childOrderStrategies { get; set; } = new();

    public double price
    {
        get
        {
            // TDA enforces 2 or 4 digits this during ReplaceOrder
            var digits = _price < 1 ? 4 : 2;
            var rounded = Math.Round(_price, digits);
            return rounded;
        }
        set => _price = value;
    }

    public double? stopPrice
    {
        get
        {
            if (_stopPrice == null)
            {
                return null;
            }
            // TDA enforces 2 or 4 digits this during ReplaceOrder
            var digits = _stopPrice < 1 ? 4 : 2;
            var rounded = Math.Round((double)_stopPrice, digits);
            return rounded;
        }
        set => _stopPrice = value;
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
    public long accountId { get; set; }
    public List<OrderActivity> orderActivityCollection { get; set; } = null!;
    public List<TDOrder> replacingOrderCollection { get; set; } = null!;
    public string statusDescription { get; set; } = null!;

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
        if (orderType != TDOrderEnums.orderType.STOP && orderType != TDOrderEnums.orderType.STOP_LIMIT && orderType != TDOrderEnums.orderType.TRAILING_STOP_LIMIT)
        {
            // Remove the stopPrice field
            var obj = JsonSerializer.Deserialize<dynamic>(json) as JsonObject;
            obj?.Remove("stopPrice");
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

    public List<object> childOrderStrategies { get; set; } = new();

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
    private double _price;

    private double? _stopPrice;

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

    /// <summary>
    ///     Must be object, not IOrderBase, to get serialization in System.Text.Json
    /// </summary>
    public List<object> childOrderStrategies { get; set; } = new();

    public double price
    {
        get
        {
            // TDA enforces 2 or 4 digits this during ReplaceOrder
            var digits = _price < 1 ? 4 : 2;
            var rounded = Math.Round(_price, digits);
            return rounded;
        }
        set => _price = value;
    }

    public double? stopPrice
    {
        get
        {
            if (_stopPrice == null)
            {
                return null;
            }
            // TDA enforces 2 or 4 digits this during ReplaceOrder
            var digits = _stopPrice < 1 ? 4 : 2;
            var rounded = Math.Round((double)_stopPrice, digits);
            return rounded;
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
            var obj = JsonSerializer.Deserialize<JsonObject>(json);
            obj?.Remove("price");
            json = JsonSerializer.Serialize(obj);
        }
        if (orderType != TDOrderEnums.orderType.STOP && orderType != TDOrderEnums.orderType.STOP_LIMIT && orderType != TDOrderEnums.orderType.TRAILING_STOP_LIMIT)
        {
            // Remove the stopPrice field
            var obj = JsonSerializer.Deserialize<JsonObject>(json);
            obj?.Remove("stopPrice");
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