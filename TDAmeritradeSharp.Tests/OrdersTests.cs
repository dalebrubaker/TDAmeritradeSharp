using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using TDAmeritradeSharpClient;

namespace TDAmeritrade.Tests;

public class OrdersTests
{
    private Client _client;
    private string _testAccountId;
    private TDEquityQuote _testQuote;

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
    public async Task Init()
    {
        // Please sign in first, following services uses the client file
        _client = new Client();
        try
        {
            await _client.RequireNotExpiredTokensAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Assert.IsTrue(false, ex.Message);
            throw;
        }
        Assert.IsTrue(_client.IsSignedIn);
        var userSettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(TDAmeritradeSharpClient));
        var testAccountPath = Path.Combine(userSettingsDirectory, "TestAccount.txt");
        _testAccountId = await File.ReadAllTextAsync(testAccountPath);

        _testQuote = await _client.GetQuote_EquityAsync("BTG"); // a low-priced stock
        Assert.IsTrue(_testQuote.Symbol == "BTG");
    }

    [Test]
    public async Task TestGetAccount()
    {
        var account = await _client.GetAccountAsync(_testAccountId);
        Assert.IsTrue(account.SecuritiesAccount.AccountId == _testAccountId);
    }

    [Test]
    public async Task TestGetAccounts()
    {
        var accounts = await _client.GetAccountsAsync();
        var testAccount = accounts.FirstOrDefault(x => x.SecuritiesAccount.AccountId == _testAccountId);
        Assert.IsNotNull(testAccount);
    }

    [Test]
    public void TestTDInstrumentConverter()
    {
        const string Symbol = "TestSymbol";
        const string UnderlyingSymbol = "TestUnderlyingSymbol";
        const string DeliverableSymbol = "TestDeliverableSymbol";
        var instrument = new InstrumentOption
        {
            Symbol = Symbol,
            Type = TDOrderEnums.TypeOption.BINARY,
            PutCall = TDOrderEnums.PutCall.CALL,
            UnderlyingSymbol = UnderlyingSymbol,
            OptionMultiplier = 1.23,
            OptionDeliverables = new List<OptionDeliverable>
            {
                new()
                {
                    Symbol = DeliverableSymbol,
                    DeliverableUnits = 123.4,
                    CurrencyType = TDOrderEnums.CurrencyType.JPY,
                    AssetType = TDOrderEnums.AssetType.MUTUAL_FUND
                },
                new()
                {
                    Symbol = DeliverableSymbol,
                    DeliverableUnits = 234.5,
                    CurrencyType = TDOrderEnums.CurrencyType.CAD,
                    AssetType = TDOrderEnums.AssetType.FIXED_INCOME
                }
            }
        };
        var json = _client.SerializeInstrument(instrument);
        Assert.IsNotEmpty(json);
        var instrumentDeserialized = (InstrumentOption)_client.DeserializeToInstrument(json);
        Assert.IsNotNull(instrumentDeserialized);
        Assert.AreEqual(instrument.UnderlyingSymbol, instrumentDeserialized.UnderlyingSymbol);
        Assert.AreEqual(instrument.OptionDeliverables[1].DeliverableUnits, instrumentDeserialized.OptionDeliverables![1].DeliverableUnits);
        var jsonSerialized = _client.SerializeInstrument(instrumentDeserialized);
        Assert.AreEqual(json, jsonSerialized);
    }

    [Test]
    public void TestTDAccountConverter()
    {
        const string AccountId = "TestAccountId";
        const int RoundTrips = 29;

        var account = new CashAccount
        {
            AccountId = AccountId,
            RoundTrips = RoundTrips,
            Positions = new List<Position>
            {
                new()
                {
                    AgedQuantity = 1.23
                },
                new()
                {
                    AgedQuantity = 2.34
                }
            },
            OrderStrategies = new List<TDOrder>
            {
                new()
                {
                    Quantity = 3.45
                },
                new()
                {
                    Quantity = 4.56
                }
            },
            InitialBalances = new InitialBalancesCash
            {
                AccountValue = 100,
                AccruedInterest = 1,
                CashAvailableForTrading = 2
            },
            ProjectedBalances = new ProjectedBalancesCash
            {
                CashBalance = 200
            }
        };
        account.OrderStrategies[0].OrderLegCollection.Add(new OrderLeg { Quantity = 123.456 });
        var json = _client.SerializeAccount(account);
        Assert.IsNotEmpty(json);
        var accountDeserialized = (CashAccount)_client.DeserializeToAccount(json);
        Assert.IsNotNull(accountDeserialized);
        var jsonSerialized = _client.SerializeAccount(accountDeserialized);
        Assert.AreEqual(json, jsonSerialized);
    }

    [Test]
    public async Task TestSingleLimitOrderSendThenCancel()
    {
        var close = _testQuote.ClosePrice;
        var limitPrice = close * 0.5; // attempt to avoid a fill
        var order = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.SINGLE,
            Price = limitPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.BUY,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        var orderId = await _client.PlaceOrderAsync(order, _testAccountId).ConfigureAwait(false);
        Assert.IsNotNull(orderId);
        var orderPlaced = await _client.GetOrderAsync(_testAccountId, orderId).ConfigureAwait(false);
        Assert.AreEqual(orderId, orderPlaced.OrderId);
        var allOrders = await _client.GetOrdersByPathAsync(_testAccountId, 2, DateTime.Today,
            status: TDOrderEnums.Status.CANCELED);
        Assert.GreaterOrEqual(allOrders.Count(), 0);
        var allOrdersQuery = await _client.GetOrdersByQueryAsync(maxResults: 2, fromEnteredTime: DateTime.Today,
            status: TDOrderEnums.Status.CANCELED);
        Assert.GreaterOrEqual(allOrdersQuery.Count(), 0);
        await _client.CancelOrderAsync(_testAccountId, orderId);
    }

    [Test]
    public async Task TestSingleLimitOrderRejected()
    {
        var close = _testQuote.ClosePrice;
        var limitPrice = close * 5; // buy limit above price s.b. rejected
        var order = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.SINGLE,
            Price = limitPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.BUY,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        var orderId = await _client.PlaceOrderAsync(order, _testAccountId).ConfigureAwait(false);
        Assert.IsNotNull(orderId);
        await Task.Delay(1000).ConfigureAwait(false);
        var orderPlaced = await _client.GetOrderAsync(_testAccountId, orderId).ConfigureAwait(false);
        Assert.AreEqual(orderId, orderPlaced.OrderId);
        Assert.AreEqual(TDOrderEnums.Status.REJECTED, orderPlaced.Status, orderPlaced.StatusDescription);

        Assert.ThrowsAsync<TDAmeritradeSharpRejectedException>(async () => await _client.CancelOrderAsync(_testAccountId, orderId), "Cancel fails on rejected order.");
    }

    [Test]
    [Ignore("Actually buys in your account at _testAccountId! Can run outside of RTH and cancel before it's filled.")]
    public async Task TestSingleMarketOrder()
    {
        var order = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.MARKET,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.SINGLE,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.BUY,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        await _client.PlaceOrderAsync(order, _testAccountId).ConfigureAwait(false);
    }

    [Test]
    public void TestCloneDeep()
    {
        var close = _testQuote.ClosePrice;
        var limitPrice = close * 0.5; // attempt to avoid a fill,
        var order = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.SINGLE,
            Price = limitPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.BUY,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        var clone = _client.CloneDeep(order);
        Assert.AreEqual(_client.SerializeOrder(order), _client.SerializeOrder(clone));
    }

    [Test]
    public async Task TestReplaceLimitOrder()
    {
        var close = _testQuote.ClosePrice;
        var limitPrice = close * 0.5; // attempt to avoid a fill
        var order = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.SINGLE,
            Price = limitPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.BUY,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        var orderId = await _client.PlaceOrderAsync(order, _testAccountId).ConfigureAwait(false);
        Assert.IsNotNull(orderId);

        var replacementOrder = _client.CloneDeep(order);
        limitPrice = close * 0.6;
        replacementOrder.Price = limitPrice;
        replacementOrder.OrderLegCollection[0].Quantity = 2;
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
        const long OrderId = 8134476058;
        var order = await _client.GetOrderAsync(_testAccountId, OrderId).ConfigureAwait(false);
        Assert.AreEqual(OrderId, order.OrderId);
    }

    [Test]
    public async Task TestSavedOrder()
    {
        var close = _testQuote.ClosePrice;
        var limitPrice = close * 0.5;
        var order = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.SINGLE,
            Price = limitPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.BUY,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        await _client.CreateSavedOrderAsync(order, _testAccountId).ConfigureAwait(false);
        var savedOrders = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        Assert.Positive(savedOrders.Count);
        var savedOrder0 = await _client.GetSavedOrderAsync(_testAccountId, savedOrders[0].SavedOrderId).ConfigureAwait(false);
        Assert.AreEqual(savedOrder0.SavedOrderId, savedOrders[0].SavedOrderId);
        await DeleteExistingSavedOrders().ConfigureAwait(false);
    }

    [Test]
    public async Task TestReplaceSavedLimitOrder()
    {
        await DeleteExistingSavedOrders().ConfigureAwait(false);

        var close = _testQuote.ClosePrice;
        var limitPrice = close * 0.5;
        var order = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.SINGLE,
            Price = limitPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.BUY,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        await _client.CreateSavedOrderAsync(order, _testAccountId).ConfigureAwait(false);
        var savedOrders = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        Assert.AreEqual(1, savedOrders.Count);
        var savedOrder0 = await _client.GetSavedOrderAsync(_testAccountId, savedOrders[0].SavedOrderId).ConfigureAwait(false);

        var replacementOrder = _client.CloneDeep(order);
        var replacementLimitPrice = close * 0.6;
        replacementOrder.Price = replacementLimitPrice;
        replacementOrder.OrderLegCollection[0].Quantity = 2;
        await _client.ReplaceSavedOrderAsync(replacementOrder, _testAccountId, savedOrder0.SavedOrderId);
        var replacementOrders = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        Assert.AreEqual(1, replacementOrders.Count);
        var replacementSavedOrder0 = await _client.GetSavedOrderAsync(_testAccountId, replacementOrders[0].SavedOrderId).ConfigureAwait(false);
        Assert.AreEqual(2, replacementSavedOrder0.OrderLegCollection[0].Quantity);
        await _client.DeleteSavedOrderAsync(_testAccountId, replacementSavedOrder0.SavedOrderId).ConfigureAwait(false);
    }

    private async Task DeleteExistingSavedOrders()
    {
        var savedOrders = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        foreach (var savedOrder in savedOrders)
        {
            await _client.DeleteSavedOrderAsync(_testAccountId, savedOrder.SavedOrderId).ConfigureAwait(false);
        }
        var savedOrders2 = (await _client.GetSavedOrdersByPathAsync(_testAccountId)).ToList();
        Assert.Zero(savedOrders2.Count);
    }

    [Test]
    public async Task OneTriggersAnotherOrderTest()
    {
        // Note that SavedOrders gives "error": "OrderStrategyType TRIGGER is not supported"
        var close = _testQuote.ClosePrice;
        var limitPrice = close * 0.5;
        var order = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.TRIGGER,
            Price = limitPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.BUY,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        var targetPrice = limitPrice + 5;
        var childOrder = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.SINGLE,
            Price = targetPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.BUY,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        order.ChildOrderStrategies = new List<TDOrder> { childOrder };
        var orderId = await _client.PlaceOrderAsync(order, _testAccountId).ConfigureAwait(false);
        await _client.CancelOrderAsync(_testAccountId, orderId);
    }

    [Test]
    public async Task OneTriggerOneCancelsAnotherOrderTest()
    {
        // Note that SavedOrders gives "error": "OrderStrategyType TRIGGER is not supported"
        var close = _testQuote.ClosePrice;
        var limitPrice = close * 0.5;
        var order = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.TRIGGER,
            Price = limitPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.BUY,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        var targetPrice = close * 2;
        var target = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            Price = targetPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.SELL,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        var priceStop = limitPrice + 0.03;
        var stop = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.STOP_LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.SINGLE,
            Price = limitPrice,
            StopPrice = priceStop,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.SELL,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        order.ChildOrderStrategies = new List<TDOrder>
        {
            target,
            stop
        };
        var orderId = await _client.PlaceOrderAsync(order, _testAccountId).ConfigureAwait(false);
        //var orders = await _client.GetOrdersByPathAsync(_testAccountId).ConfigureAwait(false);
        var order2 = await _client.GetOrderAsync(_testAccountId, orderId).ConfigureAwait(false);
        var status = order2.Status;
        if (status != TDOrderEnums.Status.REJECTED)
        {
            await _client.CancelOrderAsync(_testAccountId, orderId);
        }
    }

    [Test]
    public async Task OneCancelsAnotherOrderTest()
    {
        // Note that SavedOrders gives "error": "OrderStrategyType TRIGGER is not supported"
        var close = _testQuote.ClosePrice;
        var order = new OcoOrder();
        var targetPrice = close * 2;
        var target = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            Price = targetPrice,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.SELL,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        var price = close * .5;
        var priceStop = close * .5 + .03;
        var stop = new TDOrder
        {
            OrderType = TDOrderEnums.OrderType.STOP_LIMIT,
            Session = TDOrderEnums.Session.NORMAL,
            Duration = TDOrderEnums.Duration.DAY,
            OrderStrategyType = TDOrderEnums.OrderStrategyType.SINGLE,
            Price = price,
            StopPrice = priceStop,
            OrderLegCollection = new List<OrderLeg>
            {
                new()
                {
                    Instruction = TDOrderEnums.Instruction.SELL,
                    Quantity = 1,
                    Instrument = new InstrumentEquity
                    {
                        Symbol = _testQuote.Symbol!
                    }
                }
            }
        };
        order.ChildOrderStrategies = new List<TDOrder>
        {
            target,
            stop
        };
        var orderId = await _client.PlaceOcoOrderAsync(order, _testAccountId).ConfigureAwait(false);
        //var orders = await _client.GetOrdersByPathAsync(_testAccountId).ConfigureAwait(false);
        var order2 = await _client.GetOrderAsync(_testAccountId, orderId).ConfigureAwait(false);
        var status = order2.Status;
        if (status != TDOrderEnums.Status.REJECTED)
        {
            await _client.CancelOrderAsync(_testAccountId, orderId);
        }
    }

    [Test]
    public async Task TestGetAccountPrincipalInfo()
    {
        var account = await _client.GetAccountPrincipalInfoAsync(_testAccountId);
        Assert.AreEqual(_testAccountId, account.AccountId);
        Assert.IsNotNull(account.DisplayName);
    }

    [Test]
    public async Task TestAcctItemStreamSingleLimitOrder()
    {
        const int Timeout = 5000;
        using var socket = await GetConnectedSocket();

        var events = new List<TDRealtimeResponseContainer>();
        socket.Response += (_, response) =>
        {
            events.Add(response);
        };

        await TestSingleLimitOrderSendThenCancel().ConfigureAwait(false);
        var deadline = DateTime.UtcNow.AddMilliseconds(Timeout);
        while (events.Count == 0 && DateTime.UtcNow < deadline)
        {
            await Task.Delay(100).ConfigureAwait(false);
        }
        var elapsed = DateTime.UtcNow - deadline;
        Assert.LessOrEqual(elapsed, TimeSpan.Zero, "Timed out.");
        Assert.IsNotEmpty(events, "Expected at least one event back from TDA");

        await Task.Delay(1000).ConfigureAwait(false);
        var _ = socket.MessagesReceived;
        await socket.UnsubscribeAcctActivityAsync().ConfigureAwait(false);
        await socket.DisconnectAsync().ConfigureAwait(false);
    }

    private async Task<ClientStream> GetConnectedSocket()
    {
        var socket = new ClientStream(_client);
        await socket.Connect().ConfigureAwait(false);
        await socket.SubscribeAcctActivityAsync().ConfigureAwait(false);
        Assert.IsTrue(socket.IsConnected);
        return socket;
    }

    [Test]
    public async Task TestAcctItemStreamReplaceOrder()
    {
        const int Timeout = 5000;
        using var socket = await GetConnectedSocket();

        var events = new List<TDRealtimeResponseContainer>();
        socket.Response += (_, response) =>
        {
            events.Add(response);
        };

        await TestReplaceLimitOrder().ConfigureAwait(false);
        var deadline = DateTime.UtcNow.AddMilliseconds(Timeout);
        while (events.Count == 0 && DateTime.UtcNow < deadline)
        {
            await Task.Delay(100).ConfigureAwait(false);
        }
        var elapsed = DateTime.UtcNow - deadline;
        Assert.LessOrEqual(elapsed, TimeSpan.Zero, "Timed out.");
        Assert.IsNotEmpty(events, "Expected at least one event back from TDA");

        await Task.Delay(1000).ConfigureAwait(false);
        var _ = socket.MessagesReceived;
        await socket.UnsubscribeAcctActivityAsync().ConfigureAwait(false);
        await socket.DisconnectAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///     This test shows that the order is rejected but the OrderRejection AcctActivity message is never sent.
    /// </summary>
    [Test]
    public async Task TestAcctItemRejectedOrder()
    {
        const int Timeout = 5000;
        using var socket = await GetConnectedSocket();

        var events = new List<TDRealtimeResponseContainer>();
        socket.Response += (_, response) =>
        {
            events.Add(response);
        };

        await TestSingleLimitOrderRejected().ConfigureAwait(false);
        var deadline = DateTime.UtcNow.AddMilliseconds(Timeout);
        while (events.Count == 0 && DateTime.UtcNow < deadline)
        {
            await Task.Delay(100).ConfigureAwait(false);
        }
        var elapsed = DateTime.UtcNow - deadline;
        Assert.LessOrEqual(elapsed, TimeSpan.Zero, "Timed out.");
        Assert.IsNotEmpty(events, "Expected at least one event back from TDA");

        await Task.Delay(5000).ConfigureAwait(false);
        var _ = socket.MessagesReceived;
        await socket.UnsubscribeAcctActivityAsync().ConfigureAwait(false);
        await socket.DisconnectAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///     This test shows that the order is rejected but the OrderRejection AcctActivity message is never sent.
    /// </summary>
    [Test]
    //Ignore("Actually buys in your account at _testAccountId! Can run outside of RTH and cancel before it's filled.")]
    public async Task TestAcctItemOrderFill()
    {
        const int Timeout = 5000;
        using var socket = await GetConnectedSocket();

        var events = new List<TDRealtimeResponseContainer>();
        socket.Response += (_, response) =>
        {
            events.Add(response);
        };

        await TestSingleMarketOrder().ConfigureAwait(false);
        var deadline = DateTime.UtcNow.AddMilliseconds(Timeout);
        while (events.Count == 0 && DateTime.UtcNow < deadline)
        {
            await Task.Delay(100).ConfigureAwait(false);
        }
        var elapsed = DateTime.UtcNow - deadline;
        Assert.LessOrEqual(elapsed, TimeSpan.Zero, "Timed out.");
        Assert.IsNotEmpty(events, "Expected at least one event back from TDA");

        await Task.Delay(5000).ConfigureAwait(false);
        var _ = socket.MessagesReceived;
        await socket.UnsubscribeAcctActivityAsync().ConfigureAwait(false);
        await socket.DisconnectAsync().ConfigureAwait(false);
    }
}