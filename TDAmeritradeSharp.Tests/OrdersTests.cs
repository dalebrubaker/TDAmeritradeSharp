using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TDAmeritradeSharpClient;

namespace TDAmeritrade.Tests;

public class OrdersTests
{
    private Client _client;
    private string _testAccountId;
    private TDEquityQuote _testQuote;

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
        var userSettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(TDAmeritradeSharpClient));
        var testAccountPath = Path.Combine(userSettingsDirectory, "TestAccount.txt");
        _testAccountId = await File.ReadAllTextAsync(testAccountPath);

        _testQuote = await _client.GetQuote_Equity("BTG"); // a low-priced stock
        Assert.IsTrue(_testQuote.symbol == "BTG");
    }

    [Test]
    public async Task TestGetAccount()
    {
        var account = await _client.GetAccount(_testAccountId);
        Assert.IsTrue(account.securitiesAccount.accountId == _testAccountId);
    }

    [Test]
    public async Task TestGetAccounts()
    {
        var accounts = await _client.GetAccounts();
        var testAccount = accounts.FirstOrDefault(x => x.securitiesAccount.accountId == _testAccountId);
        Assert.IsNotNull(testAccount);
    }

    [Test]
    public async Task TestSingleLimitOrder()
    {
        var close = _testQuote.closePrice;
        var limitPrice = close * 0.5; // attempt to avoid a fill
        var order = new EquityOrder
        {
            orderType = TDOrderModelsEnums.orderType.LIMIT,
            session = TDOrderModelsEnums.session.NORMAL,
            duration = TDOrderModelsEnums.duration.DAY,
            orderStrategyType = TDOrderModelsEnums.orderStrategyType.SINGLE,
            price = limitPrice,
            OrderLeg = new EquityOrderLeg
            {
                instruction = TDOrderModelsEnums.instruction.BUY,
                quantity = 1,
                instrument = new EquityOrderInstrument
                {
                    symbol = _testQuote.symbol!
                }
            }
        };
        await _client.PlaceOrder(order, _testAccountId).ConfigureAwait(false);
    }

    [Test]
    [Ignore("Actually buys!")]
    public async Task TestSingleMarketOrder()
    {
        var order = new EquityOrder
        {
            orderType = TDOrderModelsEnums.orderType.MARKET,
            session = TDOrderModelsEnums.session.NORMAL,
            duration = TDOrderModelsEnums.duration.DAY,
            orderStrategyType = TDOrderModelsEnums.orderStrategyType.SINGLE,
            OrderLeg = new EquityOrderLeg
            {
                instruction = TDOrderModelsEnums.instruction.BUY,
                quantity = 1,
                instrument = new EquityOrderInstrument
                {
                    symbol = _testQuote.symbol!
                }
            }
        };
        await _client.PlaceOrder(order, _testAccountId).ConfigureAwait(false);
    }
}