using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using NUnit.Framework;
using Serilog;
using TDAmeritradeSharpClient.Models;

namespace TDAmeritrade.Tests;

public class AcctActivityXmlTests
{
    private static readonly ILogger s_logger = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType!);

    [OneTimeSetUp]
    public void Setup()
    {
        Tests.SetLogging();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        Log.CloseAndFlush();
    }

    [SetUp]
    public void Init()
    {
    }

    [Test]
    public void TestOrderEntryRequestMessage()
    {
        const string Response = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                + "<OrderEntryRequestMessage xmlns=\"urn:xmlns:beb.ameritrade.com\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                                + "<OrderGroupID><Firm>150</Firm><Branch>865</Branch><ClientKey>123456789</ClientKey><AccountKey>123456789</AccountKey><Segment>ngoms</Segment>"
                                + "<SubAccountType>Margin</SubAccountType><CDDomainID>A000000031539026</CDDomainID></OrderGroupID>"
                                + "<ActivityTimestamp>2022-05-03T07:20:14.36-05:00</ActivityTimestamp>"
                                + "<Order xsi:type=\"EquityOrderT\"><OrderKey>8257690153</OrderKey><Security><CUSIP>11777Q209</CUSIP><Symbol>BTG</Symbol>"
                                + "<SecurityType>Common Stock</SecurityType></Security>"
                                + "<OrderPricing xsi:type=\"LimitT\"><Ask>4.21</Ask><Bid>4.17</Bid><Limit>2.1</Limit></OrderPricing><OrderType>Limit</OrderType>"
                                + "<OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2022-05-03T07:20:14.317-05:00</OrderEnteredDateTime><OrderInstructions>Buy</OrderInstructions>"
                                + "<OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource>"
                                + "<Solicited>false</Solicited><MarketCode>Normal</MarketCode><Charges><Charge><Type>Commission Override</Type><Amount>0</Amount></Charge></Charges>"
                                + "<ClearingID>777</ClearingID><SettlementInstructions>Normal</SettlementInstructions><EnteringDevice>AA_MyAccount</EnteringDevice></Order>"
                                + "<LastUpdated>2022-05-03T07:20:14.36-05:00</LastUpdated><ConfirmTexts><ConfirmText/><ConfirmText/></ConfirmTexts></OrderEntryRequestMessage>";
        var serializer = new XmlSerializer(typeof(OrderEntryRequestMessage));
        using var reader = new StringReader(Response);
        try
        {
            var test = (OrderEntryRequestMessage)serializer.Deserialize(reader);
        }
        catch (Exception ex)
        {
            s_logger.Error(ex, "{Message}", ex.Message);
            throw;
        }
    }
}