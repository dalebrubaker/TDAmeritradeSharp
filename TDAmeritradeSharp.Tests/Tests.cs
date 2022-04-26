using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using TDAmeritradeSharpClient;

namespace TDAmeritrade.Tests;

public class Tests
{
    private Client _client;

    [SetUp]
    public async Task Init()
    {
        // Please sign in first, following services uses the client file
        _client = new Client();
        try
        {
            await _client.RequireNotExpiredTokensAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            Assert.IsTrue(false);
            throw;
        }
        Assert.IsTrue(_client.IsSignedIn);
    }

    [Test]
    public void TestTimeConverter()
    {
        var stamp1 = 1464148800000 / 1000.0;
        var time1 = TDHelpers.FromUnixTimeSeconds(stamp1);
        Assert.IsTrue(time1.Minute == 00);
        Assert.IsTrue(time1.Hour == 4);
        Assert.IsTrue(time1.Day == 25);
        Assert.IsTrue(time1.Month == 5);
        Assert.IsTrue(time1.Year == 2016);
        Assert.IsTrue(time1.DayOfWeek == DayOfWeek.Wednesday);

        var time2 = new DateTime(2016, 5, 25, 4, 0, 0, 0, DateTimeKind.Utc);
        var time3 = time2.ToEST();
        Assert.IsTrue(time3.Hour == 0);

        var stamp2 = time2.ToUnixTimeSeconds();
        var stamp3 = stamp2.UnixSecondsToMilliseconds();
        Assert.IsTrue(stamp3 == 1464148800000);
    }

    [Test]
    public async Task TestOptionChain()
    {
        var chain = await _client.GetOptionsChainAsync(new TDOptionChainRequest
        {
            symbol = "QQQ"
        });
        Assert.IsTrue(chain.callExpDateMap.Count > 0);
    }

    [Test]
    [TestCase(MarketTypes.EQUITY)]
    [TestCase(MarketTypes.OPTION)]
    public async Task TestMarketHours(MarketTypes marketType)
    {
        var hours = await _client.GetHoursForASingleMarketAsync(marketType, DateTime.Now);
        Assert.IsTrue(hours.MarketType == marketType);
    }

    [Test]
    public void TestCandleConsolidate()
    {
        var array = new TDPriceCandle[9];
        for (var i = 0; i < 9; i++)
        {
            array[i] = new TDPriceCandle { close = i, high = i, low = i, open = i, volume = i, datetime = i };
        }
        var merge1 = array.ConsolidateByTotalCount(3);
        Assert.IsTrue(merge1.Length == 3);
        var merge2 = array.ConsolidateByTotalCount(5);
        Assert.IsTrue(merge2.Length == 5);
        var merge3 = array.ConsolidateByTotalCount(1);
        Assert.IsTrue(merge3.Length == 1);

        Assert.IsTrue(merge3[0].open == 0);
        Assert.IsTrue(merge3[0].high == 8);
        Assert.IsTrue(merge3[0].low == 0);
        Assert.IsTrue(merge3[0].close == 8);
        Assert.IsTrue(merge3[0].volume == 36);
    }

    [Test]
    public void TestCandleConsolidate2()
    {
        var array = new TDPriceCandle[9];
        for (var i = 0; i < 9; i++)
        {
            array[i] = new TDPriceCandle { close = i, high = i, low = i, open = i, volume = i, datetime = i };
        }
        var merge1 = array.ConsolidateByPeriodCount(3);
        Assert.IsTrue(merge1.Length == 3);
        var merge2 = array.ConsolidateByPeriodCount(5);
        Assert.IsTrue(merge2.Length == 2);
        var merge3 = array.ConsolidateByPeriodCount(9);
        Assert.IsTrue(merge3.Length == 1);

        Assert.IsTrue(merge3[0].open == 0);
        Assert.IsTrue(merge3[0].high == 8);
        Assert.IsTrue(merge3[0].low == 0);
        Assert.IsTrue(merge3[0].close == 8);
        Assert.IsTrue(merge3[0].volume == 36);
    }

    [Test]
    public async Task TestTDQuoteClient_Equity()
    {
        var data = await _client.GetQuote_EquityAsync("MSFT");
        Assert.IsTrue(data.symbol == "MSFT");
    }

    [Test]
    public async Task TestTDQuoteClient_Index()
    {
        var data = await _client.GetQuote_IndexAsync("$SPX.X");
        Assert.IsTrue(data.symbol == "$SPX.X");
    }

    [Test]
    public async Task TestTDQuoteClient_Future()
    {
        var data = await _client.GetQuote_FutureAsync("/ES");
        Assert.IsTrue(data.symbol == "/ES");
    }

    [Test]
    public async Task TestTDQuoteClient_Option()
    {
        var data = await _client.GetQuote_OptionAsync("SPY_231215C500");
        Assert.IsTrue(data.symbol == "SPY_231215C500");
    }

    [Test]
    public async Task TestPriceHistory()
    {
        var candles = await _client.GetPriceHistoryAsync(new TDPriceHistoryRequest
        {
            // limit is 20 years of daily or 10 days of minute 
            symbol = "MSFT",
            frequencyType = TDPriceHistoryRequest.FrequencyType.minute,
            frequency = 5,
            periodType = TDPriceHistoryRequest.PeriodTypes.day,
            period = 5
        });
        Assert.IsTrue(candles.Length > 0);
    }

    [Test]
    public async Task TestPriceHistoryMaxDays()
    {
        var candles = await _client.GetPriceHistoryAsync(new TDPriceHistoryRequest
        {
            // limit is 20 years of daily or 10 days of minute 
            symbol = "MSFT",
            frequencyType = TDPriceHistoryRequest.FrequencyType.daily,
            frequency = 1,
            periodType = TDPriceHistoryRequest.PeriodTypes.year,
            period = 20
        });
        Assert.IsTrue(candles.Length > 0);
    }

    [Test]
    public async Task TestTDPrincipalClient()
    {
        //var data1 = await _client.GetUserPrincipalsAsync(TDPrincipalsFields.preferences); // gives Accounts including display names
        //var data2 = await _client.GetUserPrincipalsAsync(TDPrincipalsFields.streamerConnectionInfo);
        //var data3 = await _client.GetUserPrincipalsAsync(TDPrincipalsFields.streamerSubscriptionKeys);
        var data = await _client.GetUserPrincipalsAsync(TDPrincipalsFields.preferences, TDPrincipalsFields.streamerConnectionInfo, TDPrincipalsFields.streamerSubscriptionKeys);
        Assert.IsTrue(!string.IsNullOrEmpty(data.accessLevel));
    }

    [Test]
    public async Task TestQOSRequest()
    {
        using var socket = new ClientStream(_client);
        await socket.Connect();
        await socket.RequestQOSAsync(TDQOSLevels.FAST);
    }

    [Test]
    public async Task TestRealtimeStream()
    {
        using var socket = new ClientStream(_client);
        var symbol = "SPY";
        socket.OnHeartbeatSignal += o => { };
        socket.OnQuoteSignal += o => { };
        socket.OnTimeSaleSignal += o => { };
        socket.OnChartSignal += o => { };
        socket.OnBookSignal += o => { };
        await socket.Connect();
        await socket.SubscribeQuoteAsync(symbol);
        await socket.SubscribeChartAsync(symbol, TDChartSubs.CHART_EQUITY);
        await socket.SubscribeTimeSaleAsync(symbol, TDTimeSaleServices.TIMESALE_EQUITY);
        await socket.SubscribeBookAsync(symbol, TDBookOptions.LISTED_BOOK);
        await socket.SubscribeBookAsync(symbol, TDBookOptions.NASDAQ_BOOK);
        await Task.Delay(1000);
        Assert.IsTrue(socket.IsConnected);
        await socket.Disconnect();
    }

    [Test]
    public async Task TestRealtimeStreamFuture()
    {
        using var socket = new ClientStream(_client);
        const string Symbol = "/NQ";

        socket.OnHeartbeatSignal += o => { };
        socket.OnQuoteSignal += o => { };
        socket.OnTimeSaleSignal += o => { };
        socket.OnChartSignal += o => { };
        socket.OnBookSignal += o => { };

        await socket.Connect();
        await socket.SubscribeQuoteAsync(Symbol);
        await socket.SubscribeChartAsync(Symbol, TDChartSubs.CHART_FUTURES);
        await socket.SubscribeTimeSaleAsync(Symbol, TDTimeSaleServices.TIMESALE_FUTURES);
        await Task.Delay(2000);
        Assert.IsTrue(socket.IsConnected);
        await socket.Disconnect();
    }

    [Test]
    public void TestParser()
    {
        var reader = new TDStreamJsonProcessor();

        var counter = 5;
        reader.OnHeartbeatSignal += t =>
        {
            counter--;
        };
        reader.OnQuoteSignal += quote =>
        {
            counter--;
        };
        reader.OnTimeSaleSignal += sale =>
        {
            counter--;
        };
        reader.OnChartSignal += sale =>
        {
            counter--;
        };
        reader.OnBookSignal += sale =>
        {
            counter--;
        };

        reader.Parse("{\"notify\":[{\"heartbeat\":\"1620306966752\"}]}");
        reader.Parse(
            "{\"data\":[{\"service\":\"QUOTE\", \"timestamp\":1620306967787,\"command\":\"SUBS\",\"content\":[{\"key\":\"QQQ\",\"2\":328.75,\"4\":33,\"5\":5,\"6\":\"Q\",\"7\":\"P\",\"11\":33367}]}]}");
        reader.Parse(
            "{\"data\":[{ \"service\":\"TIMESALE_EQUITY\", \"timestamp\":1620331268678,\"command\":\"SUBS\",\"content\":[{ \"seq\":206718,\"key\":\"QQQ\",\"1\":1620331267917,\"2\":331.57,\"3\":57.0,\"4\":220028}]}]}");
        reader.Parse(
            "{\"data\":[{ \"service\":\"CHART_FUTURES\", \"timestamp\":1620348064760,\"command\":\"SUBS\",\"content\":[{ \"seq\":522,\"key\":\"/NQ\",\"1\":1620348000000,\"2\":13633.75,\"3\":13634.25,\"4\":13633.0,\"5\":13633.5,\"6\":38.0}]}]}");
        reader.Parse(
            "{\"data\":[{ \"service\":\"NASDAQ_BOOK\", \"timestamp\":1620658957880,\"command\":\"SUBS\", \"content\": [{\"key\":\"QQQ\",\"1\":1620658957722,\"2\": [{\"0\":328.47,\"1\":535,\"2\":3,\"3\":[{\"0\":\"NSDQ\",\"1\":335,\"2\":36155235}, {\"0\":\"phlx\",\"1\":100,\"2\":36157556},{\"0\":\"arcx\",\"1\":100,\"2\":36157656}]}, {\"0\":328.46,\"1\":2800,\"2\":3,\"3\":[{\"0\":\"batx\",\"1\":1000,\"2\":36157696}, {\"0\":\"nyse\",\"1\":1000,\"2\":36157697},{\"0\":\"edgx\",\"1\":800,\"2\":36157694}]}, {\"0\":328.45,\"1\":200,\"2\":2,\"3\":[{\"0\":\"MEMX\",\"1\":100,\"2\":36157694}, {\"0\":\"bosx\",\"1\":100,\"2\":36157696}]},{\"0\":328.44,\"1\":1200,\"2\":4,\"3\":[{\"0\":\"cinn\",\"1\":300,\"2\":36157339},{\"0\":\"edga\",\"1\":300,\"2\":36157555}, {\"0\":\"baty\",\"1\":300,\"2\":36157592},{\"0\":\"MIAX\",\"1\":300,\"2\":36157694}]}, {\"0\":328.42,\"1\":200,\"2\":1,\"3\":[{\"0\":\"iexg\",\"1\":200,\"2\":36157695}]}, {\"0\":327.34,\"1\":100,\"2\":1,\"3\":[{\"0\":\"mwse\",\"1\":100,\"2\":36126129}]}, {\"0\":326.77,\"1\":100,\"2\":1,\"3\":[{\"0\":\"amex\",\"1\":100,\"2\":36146149}]}], \"3\":[{\"0\":328.48,\"1\":1200,\"2\":4,\"3\":[{\"0\":\"NSDQ\",\"1\":300,\"2\":36157695}, {\"0\":\"phlx\",\"1\":300,\"2\":36157695},{\"0\":\"arcx\",\"1\":300,\"2\":36157696}, {\"0\":\"nyse\",\"1\":300,\"2\":36157696}]},{\"0\":328.49,\"1\":2800,\"2\":4,\"3\":[{\"0\":\"batx\",\"1\":1400,\"2\":36157337}, {\"0\":\"edgx\",\"1\":900,\"2\":36157695},{\"0\":\"MIAX\",\"1\":300,\"2\":36157694},{\"0\":\"MEMX\",\"1\":200,\"2\":36157694}]}, {\"0\":328.5,\"1\":300,\"2\":1,\"3\":[{\"0\":\"bosx\",\"1\":300,\"2\":36157695}]}, {\"0\":328.51,\"1\":1500,\"2\":3,\"3\":[{\"0\":\"baty\",\"1\":600,\"2\":36157337},{\"0\":\"edga\",\"1\":600,\"2\":36157695}, {\"0\":\"cinn\",\"1\":300,\"2\":36157656}]},{\"0\":328.59,\"1\":200,\"2\":1,\"3\":[{\"0\":\"iexg\",\"1\":200,\"2\":36157696}]}, {\"0\":329.55,\"1\":100,\"2\":1,\"3\":[{\"0\":\"mwse\",\"1\":100,\"2\":35431559}]}, {\"0\":336.64,\"1\":200,\"2\":1,\"3\":[{\"0\":\"GSCO\",\"1\":200,\"2\":30661019}]} ]}]}]}");
        Assert.IsTrue(counter == 0);
    }

    [Test]
    public async Task GetNewAccessTokenTests()
    {
        if (_client.AuthValues == null)
        {
            Assert.True(false);
            return;
        }
        _client.AuthValues.AccessTokenExpirationUtc = DateTime.UtcNow;
        await _client.GetNewAccessTokenAsync().ConfigureAwait(false);
        Debug.Assert(_client.AuthValues != null, "_client.AuthValues != null");
        Assert.Positive((_client.AuthValues.AccessTokenExpirationUtc - DateTime.UtcNow).Ticks);
    }

    [Test]
    public async Task GetNewRefreshTokenTests()
    {
        if (_client.AuthValues == null)
        {
            Assert.True(false);
            return;
        }
        _client.AuthValues.RefreshTokenExpirationUtc = DateTime.UtcNow;
        await _client.GetNewRefreshTokenAsync().ConfigureAwait(false);
        Debug.Assert(_client.AuthValues != null, "_client.AuthValues != null");
        Assert.Positive((_client.AuthValues.RefreshTokenExpirationUtc - DateTime.UtcNow).Ticks);
    }
}