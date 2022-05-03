using System.ComponentModel;
using System.Xml.Serialization;

// ReSharper disable InconsistentNaming
#pragma warning disable CS8618

namespace TDAmeritradeSharpClient.Models;

// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
[XmlRoot(Namespace = "urn:xmlns:beb.ameritrade.com", IsNullable = false)]
public class OrderEntryRequestMessage
{
    private DateTime activityTimestampField;

    private object[] confirmTextsField;

    private DateTime lastUpdatedField;

    private OrderEntryRequestMessageOrder orderField;

    private OrderEntryRequestMessageOrderGroupID orderGroupIDField;

    /// <remarks />
    public OrderEntryRequestMessageOrderGroupID OrderGroupID
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
    public OrderEntryRequestMessageOrder Order
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
public class OrderEntryRequestMessageOrderGroupID
{
    private uint accountKeyField;

    private ushort branchField;

    private string cDDomainIDField;

    private uint clientKeyField;

    private byte firmField;

    private string segmentField;

    private string subAccountTypeField;

    /// <remarks />
    public byte Firm
    {
        get => firmField;
        set => firmField = value;
    }

    /// <remarks />
    public ushort Branch
    {
        get => branchField;
        set => branchField = value;
    }

    /// <remarks />
    public uint ClientKey
    {
        get => clientKeyField;
        set => clientKeyField = value;
    }

    /// <remarks />
    public uint AccountKey
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
public class EquityOrderT : OrderEntryRequestMessageOrder
{
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlInclude(typeof(EquityOrderT))]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class OrderEntryRequestMessageOrder
{
    private string amountIndicatorField;

    private OrderEntryRequestMessageOrderCharges chargesField;

    private ushort clearingIDField;

    private bool discretionaryField;

    private string enteringDeviceField;

    private string marketCodeField;

    private string orderDurationField;

    private DateTime orderEnteredDateTimeField;

    private string orderInstructionsField;

    private ulong orderKeyField;

    private OrderEntryRequestMessageOrderOrderPricing orderPricingField;

    private string orderSourceField;

    private string orderTypeField;

    private byte originalQuantityField;

    private OrderEntryRequestMessageOrderSecurity securityField;

    private string settlementInstructionsField;

    private bool solicitedField;

    /// <remarks />
    public ulong OrderKey
    {
        get => orderKeyField;
        set => orderKeyField = value;
    }

    /// <remarks />
    public OrderEntryRequestMessageOrderSecurity Security
    {
        get => securityField;
        set => securityField = value;
    }

    /// <remarks />
    public OrderEntryRequestMessageOrderOrderPricing OrderPricing
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
    public byte OriginalQuantity
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
    public OrderEntryRequestMessageOrderCharges Charges
    {
        get => chargesField;
        set => chargesField = value;
    }

    /// <remarks />
    public ushort ClearingID
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
public class OrderEntryRequestMessageOrderSecurity
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
public class LimitT : OrderEntryRequestMessageOrderOrderPricing
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
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class OrderEntryRequestMessageOrderOrderPricing
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
public class OrderEntryRequestMessageOrderCharges
{
    private OrderEntryRequestMessageOrderChargesCharge chargeField;

    /// <remarks />
    public OrderEntryRequestMessageOrderChargesCharge Charge
    {
        get => chargeField;
        set => chargeField = value;
    }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "urn:xmlns:beb.ameritrade.com")]
public class OrderEntryRequestMessageOrderChargesCharge
{
    private byte amountField;

    private string typeField;

    /// <remarks />
    public string Type
    {
        get => typeField;
        set => typeField = value;
    }

    /// <remarks />
    public byte Amount
    {
        get => amountField;
        set => amountField = value;
    }
}