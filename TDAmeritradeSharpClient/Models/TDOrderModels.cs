﻿// ReSharper disable IdentifierTypo
// ReSharper disable ClassNeverInstantiated.Global
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

public class CancelTime
{
    public string Date { get; set; } = null!;
    public bool ShortFormat { get; set; }
}

public class OrderLeg
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Instruction Instruction { get; set; }

    public TDInstrument Instrument { get; set; } = null!;

    public double Quantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.OrderLegType OrderLegType { get; set; }

    public double? LegId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PositionEffect? PositionEffect { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.QuantityType? QuantityType { get; set; }

    public override string ToString()
    {
        return $"{Instrument.Symbol} {Instruction} {Quantity}";
    }
}

public class OrderActivity
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.ActivityType ActivityType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.ActivityTypeExecution ExecutionType { get; set; }

    public double Quantity { get; set; }
    public double OrderRemainingQuantity { get; set; }
    public List<ExecutionLeg> ExecutionLegs { get; set; } = null!;
}

public class ExecutionLeg
{
    public double LegId { get; set; }
    public double Quantity { get; set; }
    public double MismarkedQuantity { get; set; }
    public double Price { get; set; }
    public string Time { get; set; } = null!;
}

public class TDOrder
{
    private double _price;
    private double? _stopPrice;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.OrderType OrderType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Session Session { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Duration Duration { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.OrderStrategyType OrderStrategyType { get; set; }

    public List<OrderLeg> OrderLegCollection { get; set; } = new();

    public List<TDOrder>? ChildOrderStrategies { get; set; }

    public double Price
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

    public double? StopPrice
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

    public CancelTime CancelTime { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.ComplexOrderStrategyType? ComplexOrderStrategyType { get; set; }

    public double? Quantity { get; set; }
    public double? FilledQuantity { get; set; }
    public double? RemainingQuantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.RequestedDestination? RequestedDestination { get; set; }

    public string? DestinationLinkName { get; set; }
    public string? ReleaseTime { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopPriceLinkBasis? StopPriceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopPriceLinkType? StopPriceLinkType { get; set; }

    public double? StopPriceOffset { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopType? StopType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PriceLinkBasis? PriceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PriceLinkType? PriceLinkType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.TaxLotMethod? TaxLotMethod { get; set; }

    public double? ActivationPrice { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.SpecialInstruction? SpecialInstruction { get; set; }

    public long? OrderId { get; set; }
    public bool? Cancelable { get; set; }
    public bool? Editable { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Status? Status { get; set; }

    public string EnteredTime { get; set; } = null!;
    public string CloseTime { get; set; } = null!;
    public string? AccountId { get; set; }
    public List<OrderActivity> OrderActivityCollection { get; set; } = null!;
    public List<TDOrder> ReplacingOrderCollection { get; set; } = null!;
    public string? StatusDescription { get; set; }
}

/// <summary>
/// Use this when you only want child orders
/// </summary>
public class OcoOrder
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.OrderStrategyType OrderStrategyType => TDOrderEnums.OrderStrategyType.OCO;

    public List<TDOrder> ChildOrderStrategies { get; set; } = new();

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
    public TDOrderEnums.OrderType OrderType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Session Session { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Duration Duration { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.OrderStrategyType OrderStrategyType { get; set; }

    public List<OrderLeg> OrderLegCollection { get; set; } = new();

    public double Price { get; set; }
    public CancelTime CancelTime { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.ComplexOrderStrategyType ComplexOrderStrategyType { get; set; }

    public double Quantity { get; set; }
    public double FilledQuantity { get; set; }
    public double RemainingQuantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.RequestedDestination RequestedDestination { get; set; }

    public string DestinationLinkName { get; set; } = null!;
    public string ReleaseTime { get; set; } = null!;
    public double StopPrice { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopPriceLinkBasis StopPriceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopPriceLinkType StopPriceLinkType { get; set; }

    public double StopPriceOffset { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.StopType StopType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PriceLinkBasis PriceLinkBasis { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PriceLinkType PriceLinkType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.TaxLotMethod TaxLotMethod { get; set; }

    public double ActivationPrice { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.SpecialInstruction SpecialInstruction { get; set; }

    public long OrderId { get; set; }
    public bool Cancelable { get; set; }
    public bool Editable { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.Status Status { get; set; }

    public string EnteredTime { get; set; } = null!;
    public string CloseTime { get; set; } = null!;
    public double AccountId { get; set; }
    public List<OrderActivity> OrderActivityCollection { get; set; } = null!;
    public List<TDOrder> ReplacingOrderCollection { get; set; } = null!;
    public List<TDOrder> ChildOrderStrategies { get; set; } = null!;
    public string StatusDescription { get; set; } = null!;

    public string Tag { get; set; } = null!;

    public long SavedOrderId { get; set; }
    public string SavedTime { get; set; } = null!;

    public override string ToString()
    {
        var result = $"accountId={AccountId} orderId={OrderId} {OrderLegCollection[0]} {OrderType}";
        if (Price != 0)
        {
            result += $" {Price}";
        }
        return result;
    }
}