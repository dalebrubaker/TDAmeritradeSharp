using System.ComponentModel;
using System.Xml.Serialization;

// ReSharper disable InconsistentNaming
#pragma warning disable CS8618

namespace TDAmeritradeSharpClient;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
[XmlRoot(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
public class OrderEntryRequestMessage
{
    private DateTime activityTimestampField;

    private object[] confirmTextsField;

    private DateTime lastUpdatedField;

    private Order orderField;

    private OrderGroupID orderGroupIDField;

    /// <remarks />
    public OrderGroupID OrderGroupID
    {
        get => orderGroupIDField;
        set => orderGroupIDField = value;
    }

    /// <remarks />
    public DateTime ActivityTimestamp
    {
        get => activityTimestampField;
        set => activityTimestampField = value;
    }

    /// <remarks />
    public Order Order
    {
        get => orderField;
        set => orderField = value;
    }

    /// <remarks />
    public DateTime LastUpdated
    {
        get => lastUpdatedField;
        set => lastUpdatedField = value;
    }

    /// <remarks />
    [XmlArrayItem("ConfirmText", IsNullable = false)]
    public object[] ConfirmTexts
    {
        get => confirmTextsField;
        set => confirmTextsField = value;
    }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class OrderGroupID
{
    private string accountKeyField;

    private string branchField;

    private string cDDomainIDField;

    private string clientKeyField;

    private string firmField;

    private string segmentField;

    private string subAccountTypeField;

    /// <remarks />
    public string Firm
    {
        get => firmField;
        set => firmField = value;
    }

    /// <remarks />
    public string Branch
    {
        get => branchField;
        set => branchField = value;
    }

    /// <remarks />
    public string ClientKey
    {
        get => clientKeyField;
        set => clientKeyField = value;
    }

    /// <remarks />
    public string AccountKey
    {
        get => accountKeyField;
        set => accountKeyField = value;
    }

    /// <remarks />
    public string Segment
    {
        get => segmentField;
        set => segmentField = value;
    }

    /// <remarks />
    public string SubAccountType
    {
        get => subAccountTypeField;
        set => subAccountTypeField = value;
    }

    /// <remarks />
    public string CDDomainID
    {
        get => cDDomainIDField;
        set => cDDomainIDField = value;
    }
}

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class EquityOrderT : Order
{
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlInclude(typeof(EquityOrderT))]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class Order
{
    private string amountIndicatorField;

    private OrderCharges chargesField;

    private string clearingIDField;

    private bool discretionaryField;

    private string enteringDeviceField;

    private string marketCodeField;

    private string orderDurationField;

    private DateTime orderEnteredDateTimeField;

    private string orderInstructionsField;

    private string orderKeyField;

    private OrderPricing orderPricingField;

    private string orderSourceField;

    private string orderTypeField;

    private double originalQuantityField;

    private Security securityField;

    private string settlementInstructionsField;

    private bool solicitedField;

    /// <remarks />
    public string OrderKey
    {
        get => orderKeyField;
        set => orderKeyField = value;
    }

    /// <remarks />
    public Security Security
    {
        get => securityField;
        set => securityField = value;
    }

    /// <remarks />
    public OrderPricing OrderPricing
    {
        get => orderPricingField;
        set => orderPricingField = value;
    }

    /// <remarks />
    public string OrderType
    {
        get => orderTypeField;
        set => orderTypeField = value;
    }

    /// <remarks />
    public string OrderDuration
    {
        get => orderDurationField;
        set => orderDurationField = value;
    }

    /// <remarks />
    public DateTime OrderEnteredDateTime
    {
        get => orderEnteredDateTimeField;
        set => orderEnteredDateTimeField = value;
    }

    /// <remarks />
    public string OrderInstructions
    {
        get => orderInstructionsField;
        set => orderInstructionsField = value;
    }

    /// <remarks />
    public double OriginalQuantity
    {
        get => originalQuantityField;
        set => originalQuantityField = value;
    }

    /// <remarks />
    public string AmountIndicator
    {
        get => amountIndicatorField;
        set => amountIndicatorField = value;
    }

    /// <remarks />
    public bool Discretionary
    {
        get => discretionaryField;
        set => discretionaryField = value;
    }

    /// <remarks />
    public string OrderSource
    {
        get => orderSourceField;
        set => orderSourceField = value;
    }

    /// <remarks />
    public bool Solicited
    {
        get => solicitedField;
        set => solicitedField = value;
    }

    /// <remarks />
    public string MarketCode
    {
        get => marketCodeField;
        set => marketCodeField = value;
    }

    /// <remarks />
    public OrderCharges Charges
    {
        get => chargesField;
        set => chargesField = value;
    }

    /// <remarks />
    public string ClearingID
    {
        get => clearingIDField;
        set => clearingIDField = value;
    }

    /// <remarks />
    public string SettlementInstructions
    {
        get => settlementInstructionsField;
        set => settlementInstructionsField = value;
    }

    /// <remarks />
    public string EnteringDevice
    {
        get => enteringDeviceField;
        set => enteringDeviceField = value;
    }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class Security
{
    private string cUSIPField;

    private string securityTypeField;

    private string symbolField;

    /// <remarks />
    public string CUSIP
    {
        get => cUSIPField;
        set => cUSIPField = value;
    }

    /// <remarks />
    public string Symbol
    {
        get => symbolField;
        set => symbolField = value;
    }

    /// <remarks />
    public string SecurityType
    {
        get => securityTypeField;
        set => securityTypeField = value;
    }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class MarketT : OrderPricing
{
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class LimitT : OrderPricing
{
    private decimal limitField;

    /// <remarks />
    public decimal Limit
    {
        get => limitField;
        set => limitField = value;
    }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlInclude(typeof(LimitT))]
[XmlInclude(typeof(MarketT))]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class OrderPricing
{
    private decimal askField;

    private decimal bidField;

    /// <remarks />
    public decimal Ask
    {
        get => askField;
        set => askField = value;
    }

    /// <remarks />
    public decimal Bid
    {
        get => bidField;
        set => bidField = value;
    }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class OrderCharges
{
    private OrderChargesCharge chargeField;

    /// <remarks />
    public OrderChargesCharge Charge
    {
        get => chargeField;
        set => chargeField = value;
    }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class OrderChargesCharge
{
    private double amountField;

    private string typeField;

    /// <remarks />
    public string Type
    {
        get => typeField;
        set => typeField = value;
    }

    /// <remarks />
    public double Amount
    {
        get => amountField;
        set => amountField = value;
    }
}

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
[XmlRoot(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
public class OrderCancelRequestMessage
{
    private DateTime activityTimestampField;

    private object[] confirmTextsField;

    private DateTime lastUpdatedField;

    private Order orderField;

    private OrderGroupID orderGroupIDField;

    private double pendingCancelQuantityField;

    /// <remarks />
    public OrderGroupID OrderGroupID
    {
        get => orderGroupIDField;
        set => orderGroupIDField = value;
    }

    /// <remarks />
    public DateTime ActivityTimestamp
    {
        get => activityTimestampField;
        set => activityTimestampField = value;
    }

    /// <remarks />
    public Order Order
    {
        get => orderField;
        set => orderField = value;
    }

    /// <remarks />
    public DateTime LastUpdated
    {
        get => lastUpdatedField;
        set => lastUpdatedField = value;
    }

    /// <remarks />
    [XmlArrayItem("ConfirmText", IsNullable = false)]
    public object[] ConfirmTexts
    {
        get => confirmTextsField;
        set => confirmTextsField = value;
    }

    /// <remarks />
    public double PendingCancelQuantity
    {
        get => pendingCancelQuantityField;
        set => pendingCancelQuantityField = value;
    }
}

// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
[XmlRoot(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
public class UROUTMessage
{
    private DateTime activityTimestampField;

    private double cancelledQuantityField;

    private string internalExternalRouteIndField;

    private string orderDestinationField;

    private Order orderField;

    private OrderGroupID orderGroupIDField;

    /// <remarks />
    public OrderGroupID OrderGroupID
    {
        get => orderGroupIDField;
        set => orderGroupIDField = value;
    }

    /// <remarks />
    public DateTime ActivityTimestamp
    {
        get => activityTimestampField;
        set => activityTimestampField = value;
    }

    /// <remarks />
    public Order Order
    {
        get => orderField;
        set => orderField = value;
    }

    /// <remarks />
    public string OrderDestination
    {
        get => orderDestinationField;
        set => orderDestinationField = value;
    }

    /// <remarks />
    public string InternalExternalRouteInd
    {
        get => internalExternalRouteIndField;
        set => internalExternalRouteIndField = value;
    }

    /// <remarks />
    public double CancelledQuantity
    {
        get => cancelledQuantityField;
        set => cancelledQuantityField = value;
    }
}

[Serializable]
[DesignerCategoryAttribute("code")]
[XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
[XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
public class OrderCancelReplaceRequestMessage
{
    private DateTime activityTimestampField;

    private object[] confirmTextsField;

    private DateTime lastUpdatedField;

    private Order orderField;

    private OrderGroupID orderGroupIDField;

    private string originalOrderIdField;

    private double pendingCancelQuantityField;

    /// <remarks />
    public OrderGroupID OrderGroupID
    {
        get => orderGroupIDField;
        set => orderGroupIDField = value;
    }

    /// <remarks />
    public DateTime ActivityTimestamp
    {
        get => activityTimestampField;
        set => activityTimestampField = value;
    }

    /// <remarks />
    public Order Order
    {
        get => orderField;
        set => orderField = value;
    }

    /// <remarks />
    public DateTime LastUpdated
    {
        get => lastUpdatedField;
        set => lastUpdatedField = value;
    }

    /// <remarks />
    [XmlArrayItemAttribute("ConfirmText", IsNullable = false)]
    public object[] ConfirmTexts
    {
        get => confirmTextsField;
        set => confirmTextsField = value;
    }

    /// <remarks />
    public double PendingCancelQuantity
    {
        get => pendingCancelQuantityField;
        set => pendingCancelQuantityField = value;
    }

    /// <remarks />
    public string OriginalOrderId
    {
        get => originalOrderIdField;
        set => originalOrderIdField = value;
    }
}
[SerializableAttribute]
[DesignerCategoryAttribute("code")]
[XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
[XmlRootAttribute(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
public class OrderFillMessage
{
    private DateTime activityTimestampField;

    private object[] confirmTextsField;

    private ContraInformation contraInformationField;

    private ExecutionInformation executionInformationField;

    private double markdownAmountField;

    private double markupAmountField;

    private string orderCompletionCodeField;

    private Order orderField;

    private OrderGroupID orderGroupIDField;

    private SettlementInformation settlementInformationField;

    private decimal tradeCreditAmountField;

    private DateTime tradeDateField;

    private double trueCommCostField;

    /// <remarks />
    public OrderGroupID OrderGroupID
    {
        get => orderGroupIDField;
        set => orderGroupIDField = value;
    }

    /// <remarks />
    public DateTime ActivityTimestamp
    {
        get => activityTimestampField;
        set => activityTimestampField = value;
    }

    /// <remarks />
    public Order Order
    {
        get => orderField;
        set => orderField = value;
    }

    /// <remarks />
    public string OrderCompletionCode
    {
        get => orderCompletionCodeField;
        set => orderCompletionCodeField = value;
    }

    /// <remarks />
    public ContraInformation ContraInformation
    {
        get => contraInformationField;
        set => contraInformationField = value;
    }

    /// <remarks />
    public SettlementInformation SettlementInformation
    {
        get => settlementInformationField;
        set => settlementInformationField = value;
    }

    /// <remarks />
    public ExecutionInformation ExecutionInformation
    {
        get => executionInformationField;
        set => executionInformationField = value;
    }

    /// <remarks />
    public double MarkupAmount
    {
        get => markupAmountField;
        set => markupAmountField = value;
    }

    /// <remarks />
    public double MarkdownAmount
    {
        get => markdownAmountField;
        set => markdownAmountField = value;
    }

    /// <remarks />
    public decimal TradeCreditAmount
    {
        get => tradeCreditAmountField;
        set => tradeCreditAmountField = value;
    }

    /// <remarks />
    [XmlArrayItemAttribute("ConfirmText", IsNullable = false)]
    public object[] ConfirmTexts
    {
        get => confirmTextsField;
        set => confirmTextsField = value;
    }

    /// <remarks />
    public double TrueCommCost
    {
        get => trueCommCostField;
        set => trueCommCostField = value;
    }

    /// <remarks />
    [XmlElementAttribute(DataType = "date")]
    public DateTime TradeDate
    {
        get => tradeDateField;
        set => tradeDateField = value;
    }
}

/// <remarks />
[SerializableAttribute]
[DesignerCategoryAttribute("code")]
[XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class ContraInformation
{
    private ContraInformationContra contraField;

    /// <remarks />
    public ContraInformationContra Contra
    {
        get => contraField;
        set => contraField = value;
    }
}

/// <remarks />
[SerializableAttribute]
[DesignerCategoryAttribute("code")]
[XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class ContraInformationContra
{
    private string accountKeyField;

    private object badgeNumberField;

    private string brokerField;

    private double quantityField;

    private DateTime reportTimeField;

    private string subAccountTypeField;

    /// <remarks />
    public string AccountKey
    {
        get => accountKeyField;
        set => accountKeyField = value;
    }

    /// <remarks />
    public string SubAccountType
    {
        get => subAccountTypeField;
        set => subAccountTypeField = value;
    }

    /// <remarks />
    public string Broker
    {
        get => brokerField;
        set => brokerField = value;
    }

    /// <remarks />
    public double Quantity
    {
        get => quantityField;
        set => quantityField = value;
    }

    /// <remarks />
    public object BadgeNumber
    {
        get => badgeNumberField;
        set => badgeNumberField = value;
    }

    /// <remarks />
    public DateTime ReportTime
    {
        get => reportTimeField;
        set => reportTimeField = value;
    }
}

/// <remarks />
[SerializableAttribute]
[DesignerCategoryAttribute("code")]
[XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class SettlementInformation
{
    private string currencyField;

    private string instructionsField;

    /// <remarks />
    public string Instructions
    {
        get => instructionsField;
        set => instructionsField = value;
    }

    /// <remarks />
    public string Currency
    {
        get => currencyField;
        set => currencyField = value;
    }
}

/// <remarks />
[SerializableAttribute]
[DesignerCategoryAttribute("code")]
[XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class ExecutionInformation
{
    private bool averagePriceIndicatorField;

    private string brokerIdField;

    private string exchangeField;

    private decimal executionPriceField;

    private string idField;

    private double leavesQuantityField;

    private double quantityField;

    private DateTime timestampField;

    private string typeField;

    /// <remarks />
    public string Type
    {
        get => typeField;
        set => typeField = value;
    }

    /// <remarks />
    public DateTime Timestamp
    {
        get => timestampField;
        set => timestampField = value;
    }

    /// <remarks />
    public double Quantity
    {
        get => quantityField;
        set => quantityField = value;
    }

    /// <remarks />
    public decimal ExecutionPrice
    {
        get => executionPriceField;
        set => executionPriceField = value;
    }

    /// <remarks />
    public bool AveragePriceIndicator
    {
        get => averagePriceIndicatorField;
        set => averagePriceIndicatorField = value;
    }

    /// <remarks />
    public double LeavesQuantity
    {
        get => leavesQuantityField;
        set => leavesQuantityField = value;
    }

    /// <remarks />
    public string ID
    {
        get => idField;
        set => idField = value;
    }

    /// <remarks />
    public string Exchange
    {
        get => exchangeField;
        set => exchangeField = value;
    }

    /// <remarks />
    public string BrokerId
    {
        get => brokerIdField;
        set => brokerIdField = value;
    }
}