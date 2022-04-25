using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;


/// <summary>
/// TDInstrument is polymorphic as defined by TDA, so System.Json.Text requires that we use a TDInstrumentConverter
/// </summary>
public abstract class TDInstrument
{
    /// <summary>
    /// This is used by the <see cref="TDInstrumentConverter"/> to determine which subclass to return
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public abstract TDOrderEnums.AssetType AssetType { get; }

    public string? Cusip { get; set; }
    public string Symbol { get; set; } = "";

    public string? Description { get; set; } = null;
}

public class InstrumentEquity : TDInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override TDOrderEnums.AssetType AssetType { get; } = TDOrderEnums.AssetType.EQUITY;
}

public class InstrumentIndex : TDInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override TDOrderEnums.AssetType AssetType { get; } = TDOrderEnums.AssetType.INDEX;
}


public class InstrumentCurrency : TDInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override TDOrderEnums.AssetType AssetType { get; } = TDOrderEnums.AssetType.CURRENCY;
}

public class InstrumentFixedIncome : TDInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override TDOrderEnums.AssetType AssetType { get; } = TDOrderEnums.AssetType.FIXED_INCOME;
    public string MaturityDate { get; set; } = "";
    public double VariableRate { get; set; }
    public double Factor { get; set; }
}

public class InstrumentMutualFund : TDInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override TDOrderEnums.AssetType AssetType { get; } = TDOrderEnums.AssetType.MUTUAL_FUND;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.TypeMutualFund Type { get; set; }
}

public class InstrumentCashEquivalent : TDInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override TDOrderEnums.AssetType AssetType { get; } = TDOrderEnums.AssetType.CASH_EQUIVALENT;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.TypeCashEquivalent Type { get; set; }
}

public class InstrumentOption : TDInstrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override TDOrderEnums.AssetType AssetType { get; } = TDOrderEnums.AssetType.OPTION;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.TypeOption Type { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.PutCall PutCall { get; set; }

    public string UnderlyingSymbol { get; set; } = "";
    public double OptionMultiplier { get; set; }
    public List<OptionDeliverable>? OptionDeliverables { get; set; }
}

public class OptionDeliverable
{
    public string Symbol { get; set; } = "";
    public double DeliverableUnits { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.CurrencyType CurrencyType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.AssetType AssetType { get; set; }
}