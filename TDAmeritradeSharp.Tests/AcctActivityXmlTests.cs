using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using NUnit.Framework;
using Serilog;
using TDAmeritradeSharpClient;

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
    public void TestOrderEntryMarketRequest()
    {
        const string MessageData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                   + "<OrderEntryRequestMessage xmlns=\"urn:xmlns:beb.ameritrade.com\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                                   + "<OrderGroupID><Firm>150</Firm><Branch>253</Branch><ClientKey>253435862</ClientKey><AccountKey>253435862</AccountKey><Segment>ngoms</Segment>"
                                   + "<SubAccountType>Margin</SubAccountType><CDDomainID>A000000031539026</CDDomainID></OrderGroupID>"
                                   + "<ActivityTimestamp>2022-05-06T10:32:44.27-05:00</ActivityTimestamp><Order xsi:type=\"EquityOrderT\"><OrderKey>8293109860</OrderKey>"
                                   + "<Security><CUSIP>11777Q209</CUSIP><Symbol>BTG</Symbol><SecurityType>Common Stock</SecurityType></Security><OrderPricing xsi:type=\"MarketT\">"
                                   + "<Ask>4.38</Ask><Bid>4.37</Bid></OrderPricing><OrderType>Market</OrderType><OrderDuration>Day</OrderDuration>"
                                   + "<OrderEnteredDateTime>2022-05-06T10:32:44.244-05:00</OrderEnteredDateTime><OrderInstructions>Buy</OrderInstructions>"
                                   + "<OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary>"
                                   + "<OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode><Charges><Charge><Type>Commission Override</Type>"
                                   + "<Amount>0</Amount></Charge></Charges><ClearingID>777</ClearingID><SettlementInstructions>Normal</SettlementInstructions>"
                                   + "<EnteringDevice>AA_AnitaAndMe</EnteringDevice></Order><LastUpdated>2022-05-06T10:32:44.27-05:00</LastUpdated>"
                                   + "<ConfirmTexts><ConfirmText/><ConfirmText/></ConfirmTexts></OrderEntryRequestMessage>";
        var serializer = new XmlSerializer(typeof(OrderEntryRequestMessage));
        using var reader = new StringReader(MessageData);
        try
        {
            var test = (OrderEntryRequestMessage)serializer.Deserialize(reader);
            Assert.IsNotNull(test);
            Assert.AreEqual(4.38, ((MarketT)test.Order.OrderPricing).Ask);
        }
        catch (Exception ex)
        {
            s_logger.Error(ex, "{Message}", ex.Message);
            throw;
        }
    }
    
    [Test]
    public void TestOrderEntryLimitRequest()
    {
        const string MessageData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
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
        using var reader = new StringReader(MessageData);
        try
        {
            var test = (OrderEntryRequestMessage)serializer.Deserialize(reader);
            Assert.IsNotNull(test);
            Assert.AreEqual(2.1, ((LimitT)test.Order.OrderPricing).Limit);
        }
        catch (Exception ex)
        {
            s_logger.Error(ex, "{Message}", ex.Message);
            throw;
        }
    }

    [Test]
    public void TestOrderCancelRequest()
    {
        const string MessageData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                   + "<OrderCancelRequestMessage xmlns=\"urn:xmlns:beb.ameritrade.com\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                                   + "<OrderGroupID><Firm>150</Firm><Branch>865</Branch><ClientKey>123456789</ClientKey><AccountKey>123456789</AccountKey><Segment>ngoms</Segment>"
                                   + "<SubAccountType>Margin</SubAccountType><CDDomainID>A000000031539026</CDDomainID></OrderGroupID>"
                                   + "<ActivityTimestamp>2022-05-03T08:21:55.998-05:00</ActivityTimestamp><Order xsi:type=\"EquityOrderT\"><OrderKey>8257690647</OrderKey>"
                                   + "<Security><CUSIP>11777Q209</CUSIP><Symbol>BTG</Symbol><SecurityType>Common Stock</SecurityType></Security>"
                                   + "<OrderPricing xsi:type=\"LimitT\"><Ask>4.21</Ask><Bid>4.17</Bid><Limit>2.1</Limit></OrderPricing><OrderType>Limit</OrderType>"
                                   + "<OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2022-05-03T08:21:53.826-05:00</OrderEnteredDateTime><OrderInstructions>Buy</OrderInstructions>"
                                   + "<OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource>"
                                   + "<Solicited>false</Solicited><MarketCode>Normal</MarketCode><Charges><Charge><Type>Commission Override</Type><Amount>0</Amount></Charge></Charges>"
                                   + "<ClearingID>777</ClearingID><SettlementInstructions>Normal</SettlementInstructions><EnteringDevice>AA_MyAccount</EnteringDevice></Order>"
                                   + "<LastUpdated>2022-05-03T08:21:55.998-05:00</LastUpdated><ConfirmTexts><ConfirmText/><ConfirmText/></ConfirmTexts><PendingCancelQuantity>1</PendingCancelQuantity>"
                                   + "</OrderCancelRequestMessage>";
        var serializer = new XmlSerializer(typeof(OrderCancelRequestMessage));
        using var reader = new StringReader(MessageData);
        try
        {
            var test = (OrderCancelRequestMessage)serializer.Deserialize(reader);
            Assert.IsNotNull(test);
            Assert.AreEqual(2.1, ((LimitT)test.Order.OrderPricing).Limit);
        }
        catch (Exception ex)
        {
            s_logger.Error(ex, "{Message}", ex.Message);
            throw;
        }
    }

    [Test]
    public void TestUROUT()
    {
        const string MessageData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                   + "<UROUTMessage xmlns=\"urn:xmlns:beb.ameritrade.com\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                                   + "<OrderGroupID><Firm>150</Firm><Branch>865</Branch><ClientKey>123456789</ClientKey><AccountKey>123456789</AccountKey><Segment>ngoms</Segment>"
                                   + "<SubAccountType>Margin</SubAccountType><CDDomainID>A000000031539026</CDDomainID>"
                                   + "</OrderGroupID><ActivityTimestamp>2022-05-03T08:21:56.061-05:00</ActivityTimestamp>"
                                   + "<Order xsi:type=\"EquityOrderT\"><OrderKey>8257690647</OrderKey><Security><CUSIP>11777Q209</CUSIP><Symbol>BTG</Symbol>"
                                   + "<SecurityType>Common Stock</SecurityType></Security>"
                                   + "<OrderPricing xsi:type=\"LimitT\"><Ask>4.21</Ask><Bid>4.17</Bid><Limit>2.1</Limit></OrderPricing><OrderType>Limit</OrderType>"
                                   + "<OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2022-05-03T08:21:53.826-05:00</OrderEnteredDateTime>"
                                   + "<OrderInstructions>Buy</OrderInstructions><OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator>"
                                   + "<Discretionary>false</Discretionary><OrderSource>Web</OrderSource><Solicited>false</Solicited>"
                                   + "<MarketCode>Normal</MarketCode><ClearingID>777</ClearingID><SettlementInstructions>Normal</SettlementInstructions>"
                                   + "<EnteringDevice>AA_MyAccount</EnteringDevice></Order><OrderDestination>G1X_NMS3A</OrderDestination>"
                                   + "<InternalExternalRouteInd>True</InternalExternalRouteInd><CancelledQuantity>1</CancelledQuantity></UROUTMessage>";
        var serializer = new XmlSerializer(typeof(UROUTMessage));
        using var reader = new StringReader(MessageData);
        try
        {
            var test = (UROUTMessage)serializer.Deserialize(reader);
            Assert.IsNotNull(test);
            Assert.AreEqual(2.1, ((LimitT)test.Order.OrderPricing).Limit);
        }
        catch (Exception ex)
        {
            s_logger.Error(ex, "{Message}", ex.Message);
            throw;
        }
    }

    [Test]
    public void TestCancelReplaceRequest()
    {
        const string MessageData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                   + "<OrderCancelReplaceRequestMessage xmlns=\"urn:xmlns:beb.ameritrade.com\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                                   + "<OrderGroupID><Firm>150</Firm><Branch>865</Branch><ClientKey>123456789</ClientKey><AccountKey>123456789</AccountKey><Segment>ngoms</Segment>"
                                   + "<SubAccountType>Margin</SubAccountType><CDDomainID>A000000031539026</CDDomainID></OrderGroupID>"
                                   + "<ActivityTimestamp>2022-05-03T14:06:27.398-05:00</ActivityTimestamp>"
                                   + "<Order xsi:type=\"EquityOrderT\"><OrderKey>8264700152</OrderKey><Security><CUSIP>11777Q209</CUSIP><Symbol>BTG</Symbol>"
                                   + "<SecurityType>Common Stock</SecurityType></Security>"
                                   + "<OrderPricing xsi:type=\"LimitT\"><Ask>4.34</Ask><Bid>4.33</Bid><Limit>2.53</Limit></OrderPricing>"
                                   + "<OrderType>Limit</OrderType><OrderDuration>Day</OrderDuration><OrderEnteredDateTime>2022-05-03T14:06:27.368-05:00</OrderEnteredDateTime>"
                                   + "<OrderInstructions>Buy</OrderInstructions><OriginalQuantity>2</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary>"
                                   + "<OrderSource>Web</OrderSource><Solicited>false</Solicited><MarketCode>Normal</MarketCode>"
                                   + "<Charges><Charge><Type>Commission Override</Type><Amount>0</Amount></Charge></Charges>"
                                   + "<ClearingID>777</ClearingID><SettlementInstructions>Normal</SettlementInstructions><EnteringDevice>AA_MyAccount</EnteringDevice></Order>"
                                   + "<LastUpdated>2022-05-03T14:06:27.398-05:00</LastUpdated><ConfirmTexts><ConfirmText/><ConfirmText/></ConfirmTexts>"
                                   + "<PendingCancelQuantity>1</PendingCancelQuantity><OriginalOrderId>8264700151</OriginalOrderId></OrderCancelReplaceRequestMessage>";
        var serializer = new XmlSerializer(typeof(OrderCancelReplaceRequestMessage));
        using var reader = new StringReader(MessageData);
        try
        {
            var test = (OrderCancelReplaceRequestMessage)serializer.Deserialize(reader);
            Assert.IsNotNull(test);
            Assert.AreEqual(2.53, ((LimitT)test.Order.OrderPricing).Limit);
        }
        catch (Exception ex)
        {
            s_logger.Error(ex, "{Message}", ex.Message);
            throw;
        }
    }
    
    [Test]
    public void TestOrderFill()
    {
        const string MessageData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><OrderFillMessage xmlns=\"urn:xmlns:beb.ameritrade.com\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                                   + "<OrderGroupID><Firm>150</Firm><Branch>253</Branch><ClientKey>253435862</ClientKey><AccountKey>253435862</AccountKey><Segment>ngoms</Segment>"
                                   + "<SubAccountType>Margin</SubAccountType><CDDomainID>A000000031539026</CDDomainID></OrderGroupID>"
                                   + "<ActivityTimestamp>2022-05-06T11:04:18.633-05:00</ActivityTimestamp><Order xsi:type=\"EquityOrderT\"><OrderKey>8293987283</OrderKey>"
                                   + "<Security><CUSIP>11777Q209</CUSIP><Symbol>BTG</Symbol><SecurityType>Common Stock</SecurityType></Security>"
                                   + "<OrderPricing xsi:type=\"MarketT\"><Ask>4.37</Ask><Bid>4.36</Bid></OrderPricing><OrderType>Market</OrderType><OrderDuration>Day</OrderDuration>"
                                   + "<OrderEnteredDateTime>2022-05-06T11:04:18.467-05:00</OrderEnteredDateTime><OrderInstructions>Buy</OrderInstructions>"
                                   + "<OriginalQuantity>1</OriginalQuantity><AmountIndicator>Shares</AmountIndicator><Discretionary>false</Discretionary><OrderSource>Web</OrderSource>"
                                   + "<Solicited>false</Solicited><MarketCode>Normal</MarketCode><Charges><Charge><Type>Commission Override</Type><Amount>0</Amount></Charge>"
                                   + "<Charge><Type>SEC Fee</Type><Amount>0</Amount></Charge><Charge><Type>OR Fee</Type><Amount>0</Amount></Charge>"
                                   + "<Charge><Type>TAF Fee</Type><Amount>0</Amount></Charge></Charges><ClearingID>777</ClearingID><SettlementInstructions>Normal</SettlementInstructions>"
                                   + "<EnteringDevice>AA_AnitaAndMe</EnteringDevice></Order>"
                                   + "<OrderCompletionCode>Normal Completion</OrderCompletionCode><ContraInformation><Contra><AccountKey>253435862</AccountKey>"
                                   + "<SubAccountType>Margin</SubAccountType><Broker>FOMA2</Broker><Quantity>1</Quantity><BadgeNumber/>"
                                   + "<ReportTime>2022-05-06T11:04:18.529-05:00</ReportTime></Contra></ContraInformation><SettlementInformation><Instructions>Normal</Instructions>"
                                   + "<Currency>USD</Currency></SettlementInformation><ExecutionInformation><Type>Bought</Type>"
                                   + "<Timestamp>2022-05-06T11:04:18.529-05:00</Timestamp><Quantity>1</Quantity><ExecutionPrice>4.3656</ExecutionPrice>"
                                   + "<AveragePriceIndicator>false</AveragePriceIndicator><LeavesQuantity>0</LeavesQuantity><ID>2205069920018727549</ID><Exchange>OTC - AGENT</Exchange>"
                                   + "<BrokerId>ETMM</BrokerId></ExecutionInformation><MarkupAmount>0</MarkupAmount><MarkdownAmount>0</MarkdownAmount>"
                                   + "<TradeCreditAmount>-4.37</TradeCreditAmount><ConfirmTexts><ConfirmText/><ConfirmText/></ConfirmTexts>"
                                   + "<TrueCommCost>0</TrueCommCost><TradeDate>2022-05-06</TradeDate></OrderFillMessage>";
        var serializer = new XmlSerializer(typeof(OrderFillMessage));
        using var reader = new StringReader(MessageData);
        try
        {
            var test = (OrderFillMessage)serializer.Deserialize(reader);
            Assert.IsNotNull(test);
            Assert.AreEqual(-4.37, test.TradeCreditAmount);
            //Assert.AreEqual(2.53, ((LimitT)test.Order.OrderPricing).Limit);
        }
        catch (Exception ex)
        {
            s_logger.Error(ex, "{Message}", ex.Message);
            throw;
        }
    }
}