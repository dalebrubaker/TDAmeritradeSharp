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

        _testQuote = await _client.GetQuote_EquityAsync("BTG"); // a low-priced stock
        Assert.IsTrue(_testQuote.symbol == "BTG");
    }

    [Test]
    public async Task TestGetAccount()
    {
        var account = await _client.GetAccountAsync(_testAccountId);
        Assert.IsTrue(account.securitiesAccount.accountId == _testAccountId);
    }

    [Test]
    public async Task TestGetAccounts()
    {
        var accounts = await _client.GetAccountsAsync();
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
            priceNumeric = limitPrice,
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
        var orderId = await _client.PlaceOrderAsync(order, _testAccountId).ConfigureAwait(false);
        Assert.IsNotNull(orderId);
        var orderPlaced = await _client.GetOrderAsync(_testAccountId, orderId).ConfigureAwait(false);
        Assert.AreEqual(orderId, orderPlaced.orderId);
        var allOrders = await _client.GetOrdersByPathAsync(_testAccountId, 2, DateTime.Today,
            status: TDOrderModelsEnums.status.CANCELED);
        Assert.GreaterOrEqual(allOrders.Count(), 0);
        var allOrdersQuery = await _client.GetOrdersByQueryAsync(maxResults: 2, fromEnteredTime: DateTime.Today,
            status: TDOrderModelsEnums.status.CANCELED);
        Assert.GreaterOrEqual(allOrdersQuery.Count(), 0);
        await _client.CancelOrderAsync(_testAccountId, orderId);
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
        await _client.PlaceOrderAsync(order, _testAccountId).ConfigureAwait(false);
    }

    [Test]
    public void TestOrderBaseCloneDeep()
    {
        var close = _testQuote.closePrice;
        var order = new EquityOrder
        {
            orderType = TDOrderModelsEnums.orderType.LIMIT,
            session = TDOrderModelsEnums.session.NORMAL,
            duration = TDOrderModelsEnums.duration.DAY,
            orderStrategyType = TDOrderModelsEnums.orderStrategyType.SINGLE,
            priceNumeric = close * 0.5, // attempt to avoid a fill,
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
        var clone = order.CloneDeep();
        Assert.AreEqual(order.GetJson(), clone.GetJson());
    }

    [Test]
    public async Task TestReplaceLimitOrder()
    {
        var close = _testQuote.closePrice;
        var order = new EquityOrder
        {
            orderType = TDOrderModelsEnums.orderType.LIMIT,
            session = TDOrderModelsEnums.session.NORMAL,
            duration = TDOrderModelsEnums.duration.DAY,
            orderStrategyType = TDOrderModelsEnums.orderStrategyType.SINGLE,
            priceNumeric = close * 0.5, // attempt to avoid a fill,
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
        var orderId = await _client.PlaceOrderAsync(order, _testAccountId).ConfigureAwait(false);
        Assert.IsNotNull(orderId);

        var replacementOrder = order.CloneDeep(); // attempt to avoid a fill
        replacementOrder.priceNumeric = close * 0.6;
        replacementOrder.orderLegCollection[0].quantity = 2;
        var replacementOrderId = await _client.ReplaceOrderAsync(replacementOrder, _testAccountId, orderId);
        await _client.CancelOrderAsync(_testAccountId, replacementOrderId);
    }

    [Test]
    public async Task TestGetOrdersForAccount()
    {
        var orders = await _client.GetOrdersByPathAsync(_testAccountId).ConfigureAwait(false);
        Assert.NotNull(orders);
    }

    [Test]
    public async Task TestGetOrder()
    {
        const string OrderId = "8134476058";
        var order = await _client.GetOrderAsync(_testAccountId, OrderId).ConfigureAwait(false);
        Assert.AreEqual(OrderId, order.orderId);
    }

    [Test]
    public async Task TestSavedOrder()
    {
        var close = _testQuote.closePrice;
        var limitPrice = close * 0.5;
        var order = new EquityOrder
        {
            orderType = TDOrderModelsEnums.orderType.LIMIT,
            session = TDOrderModelsEnums.session.NORMAL,
            duration = TDOrderModelsEnums.duration.DAY,
            orderStrategyType = TDOrderModelsEnums.orderStrategyType.SINGLE,
            priceNumeric = limitPrice,
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
        await _client.CreateSavedOrderAsync(order, _testAccountId).ConfigureAwait(false);
        var savedOrders = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        Assert.Positive(savedOrders.Count);
        var savedOrder0 = await _client.GetSavedOrderAsync(_testAccountId, savedOrders[0].savedOrderId).ConfigureAwait(false);
        Assert.AreEqual(savedOrder0.savedOrderId, savedOrders[0].savedOrderId);
        foreach (var savedOrder in savedOrders)
        {
            await _client.DeleteSavedOrderAsync(_testAccountId, savedOrder.savedOrderId).ConfigureAwait(false);
        }
        var savedOrders2 = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        Assert.Zero(savedOrders2.Count);
    }

    [Test]
    public async Task TestReplaceSavedLimitOrder()
    {
        // Delete any existing ones
        var savedOrders = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        foreach (var savedOrder in savedOrders)
        {
            await _client.DeleteSavedOrderAsync(_testAccountId, savedOrder.savedOrderId).ConfigureAwait(false);
        }
        var savedOrders2 = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        Assert.Zero(savedOrders2.Count);

        var close = _testQuote.closePrice;
        var limitPrice = close * 0.5;
        var order = new EquityOrder
        {
            orderType = TDOrderModelsEnums.orderType.LIMIT,
            session = TDOrderModelsEnums.session.NORMAL,
            duration = TDOrderModelsEnums.duration.DAY,
            orderStrategyType = TDOrderModelsEnums.orderStrategyType.SINGLE,
            priceNumeric = limitPrice,
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
        await _client.CreateSavedOrderAsync(order, _testAccountId).ConfigureAwait(false);
        savedOrders = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        Assert.AreEqual(1, savedOrders.Count);
        var savedOrder0 = await _client.GetSavedOrderAsync(_testAccountId, savedOrders[0].savedOrderId).ConfigureAwait(false);

        var replacementOrder = order.CloneDeep();
        var replacementLimitPrice = close * 0.6;
        replacementOrder.priceNumeric = replacementLimitPrice;
        replacementOrder.orderLegCollection[0].quantity = 2;
        await _client.ReplaceSavedOrderAsync(replacementOrder, _testAccountId, savedOrder0.savedOrderId);
        var replacementOrders = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        Assert.AreEqual(1, replacementOrders.Count);
        var replacementSavedOrder0 = await _client.GetSavedOrderAsync(_testAccountId, replacementOrders[0].savedOrderId).ConfigureAwait(false);
        Assert.AreEqual(2, replacementSavedOrder0.orderLegCollection[0].quantity);
        await _client.DeleteSavedOrderAsync(_testAccountId, replacementSavedOrder0.savedOrderId).ConfigureAwait(false);
    }
}