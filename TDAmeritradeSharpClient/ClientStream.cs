using System.Globalization;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Web;
using Serilog;

namespace TDAmeritradeSharpClient;

/// <summary>
///     Make a request to the User Info & Preferences APIs Get User Principals method to retrieve the information found in the javascript example login request below.
///     This can be run directly in the browser console.
///     https://developer.tdameritrade.com/content/streaming-data
/// </summary>
public class ClientStream : IDisposable
{
    private static readonly ILogger s_logger = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType!);

    private readonly Client _client;
    private readonly CancellationTokenSource _cts;
    private readonly TDStreamJsonProcessor _parser;
    private readonly SemaphoreSlim _slim = new(1);
    private TDPrincipalAccount? _account;
    private int _counter;
    private TDPrincipal? _prince;
    private ClientWebSocket? _socket;

    public ClientStream(Client client)
    {
        _cts = new CancellationTokenSource();
        _client = client;
        _parser = new TDStreamJsonProcessor(this);
    }

    public JsonSerializerOptions JsonOptions => _client.JsonOptions;

    /// <summary>
    ///     Is stream connected
    /// </summary>
    public bool IsConnected => _socket == null || _socket.State == WebSocketState.Open;

    public List<string> MessagesReceived { get; } = new();

    public void Dispose()
    {
        s_logger.Verbose("Starting {Method}", nameof(Dispose));
        _cts.Cancel();
        var t = DisconnectAsync();
        t.Wait(1000);
    }

    /// <summary>
    ///     Connects to the live stream service
    /// </summary>
    /// <returns></returns>
    public async Task Connect()
    {
        try
        {
            if (!_client.IsSignedIn)
            {
                throw new Exception("Unauthorized");
            }

            if (_socket != null && _socket.State != WebSocketState.Closed)
            {
                throw new Exception("Busy");
            }
            s_logger.Verbose("Starting {Method}", nameof(Connect));
            _prince = await _client.GetUserPrincipalsAsync(TDPrincipalsFields.streamerConnectionInfo, TDPrincipalsFields.streamerSubscriptionKeys, TDPrincipalsFields.preferences);
            _account = _prince.Accounts?.Find(o => o.AccountId == _prince.PrimaryAccountId);

            var path = new Uri("wss://" + _prince.StreamerInfo?.StreamerSocketUrl + "/ws");
            _socket = new ClientWebSocket();

            await _socket.ConnectAsync(path, _cts.Token);

            if (_socket.State == WebSocketState.Open)
            {
                await LoginAsync().ConfigureAwait(false);
                ReceiveAsync();
            }
        }
        catch (Exception ex)
        {
            OnException(ex);
            Dispose();
        }
        s_logger.Verbose("Exiting {Method}", nameof(Connect));
    }

    /// <summary>
    ///     Disconnects from the live stream service and logs out
    /// </summary>
    /// <returns></returns>
    public async Task DisconnectAsync()
    {
        if (_socket != null)
        {
            if (_socket.State == WebSocketState.Open)
            {
                await LogOutAsync();
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "NormalClosure", CancellationToken.None);
                OnConnect(IsConnected);
            }
            if (_socket != null)
            {
                _socket.Dispose();
            }
            _socket = null;
        }
    }

    /// <summary>
    ///     Subscribe to the AcctActivity service
    /// </summary>
    /// <returns></returns>
    public Task SubscribeAcctActivityAsync()
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        if (_prince == null || _prince!.StreamerSubscriptionKeys == null)
        {
            throw new TDAmeritradeSharpException();
        }

        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = "ACCT_ACTIVITY",
                    Command = "SUBS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        keys = _prince?.MessageKey,
                        fields = "0,1,2,3"
                    }
                }
            }
        };
        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Unsubscribe from the AcctActivity event service
    /// </summary>
    /// <returns></returns>
    public Task UnsubscribeAcctActivityAsync()
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        if (_prince == null || _prince!.StreamerSubscriptionKeys == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var messageKey = _prince.StreamerSubscriptionKeys.Keys![0].Key;
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = "ACCT_ACTIVITY",
                    Command = "UNSUBS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        keys = _prince?.MessageKey
                    }
                }
            }
        };
        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Subscribe to the chart event service
    /// </summary>
    /// <param name="symbols">spy,qqq,amd</param>
    /// <param name="service"></param>
    /// <returns></returns>
    public Task SubscribeChartAsync(string symbols, TDChartSubs service)
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = service.ToString(),
                    Command = "SUBS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        keys = symbols,
                        fields = "0,1,2,3,4,5,6,7,8"
                    }
                }
            }
        };
        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Unsubscribe from the chart event service
    /// </summary>
    /// <param name="symbols">spy,qqq,amd</param>
    /// <param name="service"></param>
    /// <returns></returns>
    public Task UnsubscribeChartAsync(string symbols, TDChartSubs service)
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = service.ToString(),
                    Command = "UNSUBS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        keys = symbols
                    }
                }
            }
        };
        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Subscribe to the level one quote event service
    /// </summary>
    /// <param name="symbols"></param>
    /// <returns></returns>
    public Task SubscribeQuoteAsync(string symbols)
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = "QUOTE",
                    Command = "SUBS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        keys = symbols,
                        fields = "0,1,2,3,4,5,8,9,10,11,12,13,14,15,24,28"
                    }
                }
            }
        };

        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Unsubscribe from the level one quote event service
    /// </summary>
    /// <param name="symbols"></param>
    /// <returns></returns>
    public Task UnsubscribeQuoteAsync(string symbols)
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = "QUOTE",
                    Command = "UNSUBS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        keys = symbols
                    }
                }
            }
        };

        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Subscribe to the time&sales event service
    /// </summary>
    /// <param name="symbols">spy,qqq,amd</param>
    /// <param name="service">data service to subscribe to</param>
    /// <returns></returns>
    public Task SubscribeTimeSaleAsync(string symbols, TDTimeSaleServices service)
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = service.ToString(),
                    Command = "SUBS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        keys = symbols,
                        fields = "0,1,2,3,4"
                    }
                }
            }
        };

        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Unsubscribe from the time&sales event service
    /// </summary>
    /// <param name="symbols">spy,qqq,amd</param>
    /// <param name="service">data service to subscribe to</param>
    /// <returns></returns>
    public Task UnsubscribeTimeSaleAsync(string symbols, TDTimeSaleServices service)
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = service.ToString(),
                    Command = "UNSUBS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        keys = symbols
                    }
                }
            }
        };

        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Subscribe to the level two order book. Note this stream has no official documentation, and it's not entirely clear what exchange it corresponds to.Use at your own risk.
    /// </summary>
    /// <returns></returns>
    public Task SubscribeBookAsync(string symbols, TDBookOptions option)
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = option.ToString(),
                    Command = "SUBS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        keys = symbols,
                        fields = "0,1,2,3"
                    }
                }
            }
        };

        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Unsubscribe from the level two order book. Note this stream has no official documentation, and it's not entirely clear what exchange it corresponds to.Use at your own risk.
    /// </summary>
    /// <returns></returns>
    public Task UnsubscribeBookAsync(string symbols, TDBookOptions option)
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = option.ToString(),
                    Command = "UNSUBS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        keys = symbols
                    }
                }
            }
        };

        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Quality of Service provides the different rates of data updates per protocol (binary, websocket etc), or per user based.
    /// </summary>
    /// <param name="quality">Quality of Service, or the rate the data will be sent to the client.</param>
    /// <returns></returns>
    public Task RequestQOSAsync(TDQOSLevels quality)
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = "ADMIN",
                    Command = "QOS",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        qoslevel = (int)quality
                    }
                }
            }
        };

        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Sends a request to the server
    /// </summary>
    /// <param name="data"></param>
    private async Task SendToServerAsync(string data)
    {
        await _slim.WaitAsync();
        try
        {
            if (_socket != null)
            {
                var encoded = Encoding.UTF8.GetBytes(data);
                var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
                await _socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            OnException(ex);
            Dispose();
        }
        finally
        {
            _slim.Release();
        }
    }

    //

    private async void ReceiveAsync()
    {
        s_logger.Verbose("Starting {Method}", nameof(ReceiveAsync));
        var buffer = new ArraySegment<byte>(new byte[2048]);
        try
        {
            do
            {
                WebSocketReceiveResult result;
                using var ms = new MemoryStream();
                do
                {
                    result = await _socket?.ReceiveAsync(buffer, CancellationToken.None)!;
                    if (buffer.Array != null)
                    {
                        // s_logger.Debug("Writing {Count} bytes in {Method}", result.Count, nameof(ReceiveAsync));
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                } while (!result.EndOfMessage && !_cts.IsCancellationRequested);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    // s_logger.Debug("WebSocketMessageType.Close in {Method}", nameof(ReceiveAsync));
                    break;
                    //throw new Exception("WebSocketMessageType.Close");
                }

                ms.Seek(0, SeekOrigin.Begin);

                using var reader = new StreamReader(ms, Encoding.UTF8);
                var msg = await reader.ReadToEndAsync();
                HandleMessage(msg);
            } while (_socket != null && _socket.State == WebSocketState.Open);
        }
        catch (Exception ex)
        {
            OnException(ex);
            Dispose();
        }
        s_logger.Verbose("Exiting {Method}", nameof(ReceiveAsync));
    }

    private Task LoginAsync()
    {
        if (_account == null)
        {
            return Task.CompletedTask;
        }

        s_logger.Verbose("Starting {Method}", nameof(LoginAsync));

        //Converts ISO-8601 response in snapshot to ms since epoch accepted by Streamer
        var tokenTimeStampAsDateObj = DateTime.Parse(_prince?.StreamerInfo?.TokenTimestamp);
        var tokenTimeStampAsMs = tokenTimeStampAsDateObj.ToUniversalTime().ToUnixTimeMilliseconds();

        var queryString = HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("userid", _account.AccountId);
        queryString.Add("company", _account.Company);
        queryString.Add("segment", _account.Segment);
        queryString.Add("cddomain", _account.AccountCdDomainId);

        queryString.Add("token", _prince?.StreamerInfo?.Token);
        queryString.Add("usergroup", _prince?.StreamerInfo?.UserGroup);
        queryString.Add("accessLevel", _prince?.StreamerInfo?.AccessLevel);
        queryString.Add("appId", _prince?.StreamerInfo?.AppId);
        queryString.Add("acl", _prince?.StreamerInfo?.Acl);

        queryString.Add("timestamp", tokenTimeStampAsMs.ToString(CultureInfo.InvariantCulture));
        queryString.Add("authorized", "Y");

        var credits = queryString.ToString();
        var encoded = HttpUtility.UrlEncode(credits);

        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = "ADMIN",
                    Command = "LOGIN",
                    Requestid = Interlocked.Increment(ref _counter),
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new
                    {
                        token = _prince?.StreamerInfo?.Token,
                        version = "1.0",
                        credential = encoded
                    }
                }
            }
        };
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var data = JsonSerializer.Serialize(request, options);
        var result = SendToServerAsync(data);
        s_logger.Verbose("Exiting {Method}", nameof(LoginAsync));
        return result;
    }

    private Task LogOutAsync()
    {
        if (_account == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var request = new TDRealtimeRequestContainer
        {
            Requests = new[]
            {
                new TDRealtimeRequest
                {
                    Service = "ADMIN",
                    Command = "LOGOUT",
                    Requestid = Interlocked.Increment(ref _counter), // CANNOT BE RequestId!
                    Account = _account.AccountId,
                    Source = _prince?.StreamerInfo?.AppId,
                    Parameters = new { }
                }
            }
        };
        var data = JsonSerializer.Serialize(request, JsonOptions);
        return SendToServerAsync(data);
    }

    /// <summary>
    ///     Handle each received message by throwing it to OnJsonSignal then sending it to _parser,
    ///     which will throw appropriate events
    /// </summary>
    /// <param name="msg"></param>
    private void HandleMessage(string msg)
    {
        MessagesReceived.Add(msg);
        try
        {
            s_logger.Verbose("{Method}: {Msg}", nameof(HandleMessage), msg);
            OnJsonSignal(msg);
            _parser.Parse(msg);
        }
        catch (Exception ex)
        {
            OnException(ex);
            //Do not cleanup, this is a user code issue
        }
    }

    /// <summary>Client sent errors</summary>
    public event EventHandler<Exception>? ExceptionEvent;

    private void OnException(Exception signal)
    {
        var tmp = ExceptionEvent; // for thread safety
        tmp?.Invoke(this, signal);
    }

    /// <summary> Server Sent Events </summary>
    public event EventHandler<bool>? ConnectEvent;

    private void OnConnect(bool signal)
    {
        var tmp = ConnectEvent; // for thread safety
        tmp?.Invoke(this, signal);
    }

    /// <summary> Server Sent Events as raw json </summary>
    public event EventHandler<string>? JsonSignal;

    private void OnJsonSignal(string signal)
    {
        var tmp = JsonSignal; // for thread safety
        tmp?.Invoke(this, signal);
    }

    /// <summary> Server Sent Events </summary>
    public event EventHandler<TDHeartbeatSignal>? HeartbeatSignal;

    internal void OnHeartbeatSignal(TDHeartbeatSignal signal)
    {
        var tmp = HeartbeatSignal; // for thread safety
        tmp?.Invoke(this, signal);
    }

    /// <summary> Server Sent Events </summary>
    public event EventHandler<TDChartSignal>? ChartSignal;

    internal void OnChartSignal(TDChartSignal signal)
    {
        var tmp = ChartSignal; // for thread safety
        tmp?.Invoke(this, signal);
    }

    /// <summary> Server Sent Events </summary>
    public event EventHandler<TDQuoteSignal>? QuoteSignal;

    internal void OnQuoteSignal(TDQuoteSignal signal)
    {
        var tmp = QuoteSignal; // for thread safety
        tmp?.Invoke(this, signal);
    }

    /// <summary> Server Sent Events </summary>
    public event EventHandler<TDTimeSaleSignal>? TimeSaleSignal;

    internal void OnTimeSaleSignal(TDTimeSaleSignal signal)
    {
        var tmp = TimeSaleSignal; // for thread safety
        tmp?.Invoke(this, signal);
    }

    /// <summary> Server Sent Events </summary>
    public event EventHandler<TDBookSignal>? BookSignal;

    internal void OnBookSignal(TDBookSignal bookSignal)
    {
        var tmp = BookSignal; // for thread safety
        tmp?.Invoke(this, bookSignal);
    }

    /// <summary>
    ///     A response node came from the stream
    /// </summary>
    public event EventHandler<TDRealtimeResponseContainer>? Response;

    public void OnResponse(TDRealtimeResponseContainer response)
    {
        var tmp = Response; // for thread safety
        tmp?.Invoke(this, response);
    }

    public event EventHandler<OrderEntryRequestMessage>? OrderEntryRequest;

    internal void OnOrderEntryRequest(OrderEntryRequestMessage orderEntryRequest)
    {
        var tmp = OrderEntryRequest; // for thread safety
        tmp?.Invoke(this, orderEntryRequest);
    }

    public event EventHandler<OrderCancelRequestMessage>? OrderCancelRequest;

    internal void OnOrderCancelRequest(OrderCancelRequestMessage orderCancelRequest)
    {
        var tmp = OrderCancelRequest; // for thread safety
        tmp?.Invoke(this, orderCancelRequest);
    }

    public event EventHandler<UROUTMessage>? UROUTMessage;

    internal void OnUROUTMessage(UROUTMessage uroutMessage)
    {
        var tmp = UROUTMessage; // for thread safety
        tmp?.Invoke(this, uroutMessage);
    }

    public event EventHandler<OrderCancelReplaceRequestMessage>? OrderCancelReplaceRequest;

    internal void OnOrderCancelReplaceRequestMessage(OrderCancelReplaceRequestMessage cancelReplaceRequest)
    {
        var tmp = OrderCancelReplaceRequest; // for thread safety
        tmp?.Invoke(this, cancelReplaceRequest);
    }
}