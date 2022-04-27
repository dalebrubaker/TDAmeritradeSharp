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
    public TDOrderEnums.Instruction instruction { get; set; }

    public TDInstrument instrument { get; set; } = null!;

    public double quantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.OrderLegType orderLegType { get; set; }

    public double legId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PositionEffect positionEffect { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.QuantityType quantityType { get; set; }

    public override string ToString()
    {
        return $"{instrument.Symbol} {instruction} {quantity}";
    }
}

public class OrderActivity
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.ActivityType activityType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.ActivityTypeExecution executionType { get; set; }

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
    public TDOrderEnums.OrderType orderType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Session session { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Duration duration { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.OrderStrategyType orderStrategyType { get; set; }

    public List<OrderLeg> orderLegCollection { get; set; } = new();

    public List<TDOrder> childOrderStrategies { get; set; } = new();

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
    public TDOrderEnums.ComplexOrderStrategyType complexOrderStrategyType { get; set; }

    public double quantity { get; set; }
    public double filledQuantity { get; set; }
    public double remainingQuantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.RequestedDestination requestedDestination { get; set; }

    public string destinationLinkName { get; set; } = null!;
    public string releaseTime { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopPriceLinkBasis stopPriceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopPriceLinkType stopPriceLinkType { get; set; }

    public double stopPriceOffset { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopType stopType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PriceLinkBasis priceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PriceLinkType priceLinkType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.TaxLotMethod taxLotMethod { get; set; }

    public double activationPrice { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.SpecialInstruction specialInstruction { get; set; }

    public long orderId { get; set; }
    public bool cancelable { get; set; }
    public bool editable { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Status status { get; set; }

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
        if (orderType == TDOrderEnums.OrderType.MARKET)
        {
            // Remove the price field
            var obj = JsonSerializer.Deserialize<dynamic>(json) as JsonObject;
            obj?.Remove("price");
            json = JsonSerializer.Serialize(obj);
        }
        if (orderType != TDOrderEnums.OrderType.STOP && orderType != TDOrderEnums.OrderType.STOP_LIMIT && orderType != TDOrderEnums.OrderType.TRAILING_STOP_LIMIT)
        {
            // Remove the stopPrice field
            var obj = JsonSerializer.Deserialize<dynamic>(json) as JsonObject;
            obj?.Remove("stopPrice");
            json = JsonSerializer.Serialize(obj);
        }
        return json;
    }
}

public class OcoOrder
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.OrderStrategyType orderStrategyType => TDOrderEnums.OrderStrategyType.OCO;

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



public class TDOrderResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.OrderType orderType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Session session { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Duration duration { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.OrderStrategyType orderStrategyType { get; set; }

    public List<OrderLeg> orderLegCollection { get; set; } = new();

    public double price { get; set; }
    public CancelTime cancelTime { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.ComplexOrderStrategyType complexOrderStrategyType { get; set; }

    public double quantity { get; set; }
    public double filledQuantity { get; set; }
    public double remainingQuantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.RequestedDestination requestedDestination { get; set; }

    public string destinationLinkName { get; set; } = null!;
    public string releaseTime { get; set; } = null!;
    public double stopPrice { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopPriceLinkBasis stopPriceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopPriceLinkType stopPriceLinkType { get; set; }

    public double stopPriceOffset { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopType stopType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PriceLinkBasis priceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PriceLinkType priceLinkType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.TaxLotMethod taxLotMethod { get; set; }

    public double activationPrice { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.SpecialInstruction specialInstruction { get; set; }

    public long orderId { get; set; }
    public bool cancelable { get; set; }
    public bool editable { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Status status { get; set; }

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