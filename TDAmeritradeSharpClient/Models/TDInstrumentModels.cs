using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

public class Instrument
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TDOrderEnums.assetType assetType { get; set; }

    public string? cusip { get; set; } = null!;
    public string symbol { get; set; } = "";

    public string? description { get; set; } = null;
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