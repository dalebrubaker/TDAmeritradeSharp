using System.Collections.Specialized;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TDAmeritradeSharpClient;

/// <summary>
///     Make a request to the User Info & Preferences APIs Get User Principals method to retrieve the information found in the javascript example login request below.
///     This can be run directly in the browser console.
///     https://developer.tdameritrade.com/content/streaming-data
/// </summary>
public class ClientStream : IDisposable
{
    private readonly Client _client;
    private readonly TDStreamJsonProcessor _parser;
    private readonly JsonSerializerSettings _settings = new() { NullValueHandling = NullValueHandling.Ignore };
    private readonly SemaphoreSlim _slim = new(1);
    private TDAccount? _account;
    private bool _connected;
    private int _counter;
    private TDPrincipal? _prince;
    private ClientWebSocket? _socket;

    public ClientStream(Client client)
    {
        _client = client;
        _parser = new TDStreamJsonProcessor();
        _parser.OnHeartbeatSignal += o => { OnHeartbeatSignal(o); };
        _parser.OnChartSignal += o => { OnChartSignal(o); };
        _parser.OnQuoteSignal += o => { OnQuoteSignal(o); };
        _parser.OnTimeSaleSignal += o => { OnTimeSaleSignal(o); };
        _parser.OnBookSignal += o => { OnBookSignal(o); };
    }

    /// <summary>
    ///     Is stream connected
    /// </summary>
    public bool IsConnected
    {
        get => _connected;
        private set
        {
            if (_connected == value)
            {
                return;
            }
            _connected = value;
            OnConnect(value);
        }
    }

    public void Dispose()
    {
        CleanupAsync();
    }

    /// <summary>Client sent errors</summary>
    public event Action<Exception> OnException = delegate { };

    /// <summary> Server Sent Events </summary>
    public event Action<bool> OnConnect = delegate { };

    /// <summary> Server Sent Events as raw json </summary>
    public event Action<string> OnJsonSignal = delegate { };

    /// <summary> Server Sent Events </summary>
    public event Action<TDHeartbeatSignal> OnHeartbeatSignal = delegate { };

    /// <summary> Server Sent Events </summary>
    public event Action<TDChartSignal> OnChartSignal = delegate { };

    /// <summary> Server Sent Events </summary>
    public event Action<TDQuoteSignal> OnQuoteSignal = delegate { };

    /// <summary> Server Sent Events </summary>
    public event Action<TDTimeSaleSignal> OnTimeSaleSignal = delegate { };

    /// <summary> Server Sent Events </summary>
    public event Action<TDBookSignal> OnBookSignal = delegate { };

    /// <summary>
    /// Connects to the live stream service
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

            _prince = await _client.GetUserPrincipalsAsync(TDPrincipalsFields.streamerConnectionInfo, TDPrincipalsFields.streamerSubscriptionKeys, TDPrincipalsFields.preferences);
            _account = _prince.accounts?.Find(o => o.accountId == _prince.primaryAccountId);

            var path = new Uri("wss://" + _prince.streamerInfo?.streamerSocketUrl + "/ws");
            _socket = new ClientWebSocket();

            await _socket.ConnectAsync(path, CancellationToken.None);

            if (_socket.State == WebSocketState.Open)
            {
                await LoginAsync();
                ReceiveAsync();
                IsConnected = true;
            }
        }
        catch (Exception ex)
        {
            OnException(ex);
            CleanupAsync();
        }
    }

    /// <summary>
    /// Disconnects from the live stream service and logs out
    /// </summary>
    /// <returns></returns>
    public async Task Disconnect()
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
        IsConnected = false;
    }

    /// <summary>
    /// Subscribe to the chart event service
    /// </summary>
    /// <param name="symbols">spy,qqq,amd</param>
    /// <param name="service"></param>
    /// <returns></returns>
    public Task SubscribeChartAsync(string symbols, TDChartSubs service)
    {
        var request = new TDRealtimeRequestContainer
        {
            requests = new TDRealtimeRequest[]
            {
                new TDRealtimeRequest
                {
                    service = service.ToString(),
                    command = "SUBS",
                    requestid = Interlocked.Increment(ref _counter),
                    account = _account?.accountId,
                    source = _prince?.streamerInfo?.appId,
                    parameters = new
                    {
                        keys = symbols,
                        fields = "0,1,2,3,4,5,6,7,8"
                    }
                }
            }
        };
        var data = JsonConvert.SerializeObject(request, _settings);
        return SendToServerAsync(data);    }

        /// <summary>
        /// Unsubscribe from the chart event service
        /// </summary>
        /// <param name="symbols">spy,qqq,amd</param>
        /// <param name="service"></param>
        /// <returns></returns>
        public Task UnsubscribeChartAsync(string symbols, TDChartSubs service)
        {
            var request = new TDRealtimeRequestContainer
            {
                requests = new TDRealtimeRequest[]
                    {
                    new TDRealtimeRequest
                    {
                        service = service.ToString(),
                        command = "UNSUBS",
                        requestid = Interlocked.Increment(ref _counter),
                        account = _account?.accountId,
                        source = _prince?.streamerInfo?.appId,
                        parameters = new
                        {
                            keys = symbols,
                        }
                    }
                    }
            };
            var data = JsonConvert.SerializeObject(request, _settings);
            return SendToServerAsync(data);
        }

        /// <summary>
        /// Subscribe to the level one quote event service
        /// </summary>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public Task SubscribeQuoteAsync(string symbols)
        {
            var request = new TDRealtimeRequestContainer
            {
                requests = new TDRealtimeRequest[]
                {
                    new TDRealtimeRequest
                    {
                        service = "QUOTE",
                        command = "SUBS",
                        requestid = Interlocked.Increment(ref _counter),
                        account = _account?.accountId,
                        source = _prince?.streamerInfo?.appId,
                        parameters = new
                        {
                            keys = symbols,
                            fields = "0,1,2,3,4,5,8,9,10,11,12,13,14,15,24,28"
                        }
                    }
                }
            };

            var data = JsonConvert.SerializeObject(request, _settings);
            return SendToServerAsync(data);
        }

        /// <summary>
        /// Unsubscribe from the level one quote event service
        /// </summary>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public Task UnsubscribeQuoteAsync(string symbols)
        {
            var request = new TDRealtimeRequestContainer
            {
                requests = new TDRealtimeRequest[]
                {
                    new TDRealtimeRequest
                    {
                        service = "QUOTE",
                        command = "UNSUBS",
                        requestid = Interlocked.Increment(ref _counter),
                        account = _account?.accountId,
                        source = _prince?.streamerInfo?.appId,
                        parameters = new
                        {
                            keys = symbols,
                        }
                    }
                }
            };

            var data = JsonConvert.SerializeObject(request, _settings);
            return SendToServerAsync(data);
        }

        /// <summary>
        /// Subscribe to the time&sales event service
        /// </summary>
        /// <param name="symbols">spy,qqq,amd</param>
        /// <param name="service">data service to subscribe to</param>
        /// <returns></returns>
        public Task SubscribeTimeSaleAsync(string symbols, TDTimeSaleServices service)
        {
            var request = new TDRealtimeRequestContainer
            {
                requests = new TDRealtimeRequest[]
                {
                    new TDRealtimeRequest
                    {
                        service = service.ToString(),
                        command = "SUBS",
                        requestid = Interlocked.Increment(ref _counter),
                        account = _account?.accountId,
                        source = _prince?.streamerInfo?.appId,
                        parameters = new
                        {
                            keys = symbols,
                            fields = "0,1,2,3,4"
                        }
                    }
                }
            };

            var data = JsonConvert.SerializeObject(request, _settings);
            return SendToServerAsync(data);
        }

        /// <summary>
        /// Unsubscribe from the time&sales event service
        /// </summary>
        /// <param name="symbols">spy,qqq,amd</param>
        /// <param name="service">data service to subscribe to</param>
        /// <returns></returns>
        public Task UnsubscribeTimeSaleAsync(string symbols, TDTimeSaleServices service)
        {
            var request = new TDRealtimeRequestContainer
            {
                requests = new TDRealtimeRequest[]
                {
                    new TDRealtimeRequest
                    {
                        service = service.ToString(),
                        command = "UNSUBS",
                        requestid = Interlocked.Increment(ref _counter),
                        account = _account?.accountId,
                        source = _prince?.streamerInfo?.appId,
                        parameters = new
                        {
                            keys = symbols,
                        }
                    }
                }
            };

            var data = JsonConvert.SerializeObject(request, _settings);
            return SendToServerAsync(data);
        }

        /// <summary>
        /// Subscribe to the level two order book. Note this stream has no official documentation, and it's not entirely clear what exchange it corresponds to.Use at your own risk.
        /// </summary>
        /// <returns></returns>
        public Task SubscribeBookAsync(string symbols, TDBookOptions option)
        {
            var request = new TDRealtimeRequestContainer
            {
                requests = new TDRealtimeRequest[]
                {
                    new TDRealtimeRequest
                    {
                        service = option.ToString(),
                        command = "SUBS",
                        requestid = Interlocked.Increment(ref _counter),
                        account = _account?.accountId,
                        source = _prince?.streamerInfo?.appId,
                        parameters = new
                        {
                            keys = symbols,
                            fields = "0,1,2,3"
                        }
                    }
                }
            };

            var data = JsonConvert.SerializeObject(request, _settings);
            return SendToServerAsync(data);
        }

        /// <summary>
        /// Unsubscribe from the level two order book. Note this stream has no official documentation, and it's not entirely clear what exchange it corresponds to.Use at your own risk.
        /// </summary>
        /// <returns></returns>
        public Task UnsubscribeBookAsync(string symbols, TDBookOptions option)
        {
            var request = new TDRealtimeRequestContainer
            {
                requests = new TDRealtimeRequest[]
                {
                    new TDRealtimeRequest
                    {
                        service = option.ToString(),
                        command = "UNSUBS",
                        requestid = Interlocked.Increment(ref _counter),
                        account = _account?.accountId,
                        source = _prince?.streamerInfo?.appId,
                        parameters = new
                        {
                            keys = symbols,
                        }
                    }
                }
            };

            var data = JsonConvert.SerializeObject(request, _settings);
            return SendToServerAsync(data);
        }

        /// <summary>
        /// Quality of Service provides the different rates of data updates per protocol (binary, websocket etc), or per user based.
        /// </summary>
        /// <param name="quality">Quality of Service, or the rate the data will be sent to the client.</param>
        /// <returns></returns>
        public Task RequestQOSAsync(TDQOSLevels quality)
        {
            var request = new TDRealtimeRequestContainer
            {
                requests = new TDRealtimeRequest[]
                {
                    new TDRealtimeRequest
                    {
                        service = "ADMIN",
                        command = "QOS",
                        requestid = Interlocked.Increment(ref _counter),
                        account = _account?.accountId,
                        source = _prince?.streamerInfo?.appId,
                        parameters = new
                        {
                            qoslevel = ((int)quality)
                        }
                    }
                }
            };

            var data = JsonConvert.SerializeObject(request, _settings);
            return SendToServerAsync(data);
        }

        /// <summary>
        /// Sends a request to the server
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
                CleanupAsync();
            }
            finally
            {
                _slim.Release();
            }
        }

    //

    private async void ReceiveAsync()
    {
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
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    throw new Exception("WebSocketMessageType.Close");
                }

                ms.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    var msg = await reader.ReadToEndAsync();
                    HandleMessage(msg);
                }
            } while (_socket != null && _socket.State == WebSocketState.Open);
        }
        catch (Exception ex)
        {
            OnException(ex);
            CleanupAsync();
        }
    }

    private Task LoginAsync()
        {
            if (_account == null)
            {
                return Task.CompletedTask;
            }
            //Converts ISO-8601 response in snapshot to ms since epoch accepted by Streamer
            var tokenTimeStampAsDateObj = DateTime.Parse(_prince?.streamerInfo?.tokenTimestamp);
            var tokenTimeStampAsMs = tokenTimeStampAsDateObj.ToUniversalTime().ToUnixTimeMilliseconds();

            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

            queryString.Add("userid", _account.accountId);
            queryString.Add("company", _account.company);
            queryString.Add("segment", _account.segment);
            queryString.Add("cddomain", _account.accountCdDomainId);

            queryString.Add("token", _prince?.streamerInfo?.token);
            queryString.Add("usergroup", _prince?.streamerInfo?.userGroup);
            queryString.Add("accessLevel", _prince?.streamerInfo?.accessLevel);
            queryString.Add("appId", _prince?.streamerInfo?.appId);
            queryString.Add("acl", _prince?.streamerInfo?.acl);

            queryString.Add("timestamp", tokenTimeStampAsMs.ToString(CultureInfo.InvariantCulture));
            queryString.Add("authorized", "Y");

            var credits = queryString.ToString();
            var encoded = HttpUtility.UrlEncode(credits);

            var request = new TDRealtimeRequestContainer
            {
                requests = new TDRealtimeRequest[]
                {
                    new TDRealtimeRequest
                    {
                        service = "ADMIN",
                        command = "LOGIN",
                        requestid = Interlocked.Increment(ref _counter),
                        account = _account.accountId,
                        source = _prince?.streamerInfo?.appId,
                        parameters = new
                        {
                            token = _prince?.streamerInfo?.token,
                            version = "1.0",
                            credential = encoded,
                        }
                    }
                }
            };
            var data = JsonConvert.SerializeObject(request);
            return SendToServerAsync(data);
        }

        private Task LogOutAsync()
        {
            var request = new TDRealtimeRequestContainer
            {
                requests = new TDRealtimeRequest[]
                {
                    new TDRealtimeRequest
                    {
                        service = "ADMIN",
                        command = "LOGOUT",
                        requestid = Interlocked.Increment(ref _counter),
                        account = _account?.accountId,
                        source = _prince?.streamerInfo?.appId,
                        parameters = new { }
                    }
                }
            };
            var data = JsonConvert.SerializeObject(request);
            return SendToServerAsync(data);
        }

    private void HandleMessage(string msg)
    {
        try
        {
            OnJsonSignal(msg);
            _parser.Parse(msg);
        }
        catch (Exception ex)
        {
            OnException(ex);
            //Do not cleanup, this is a user code issue
        }
    }

    private async void CleanupAsync()
    {
        if (_socket != null)
        {
            if (_socket.State == WebSocketState.Open)
            {
                await LogOutAsync();
                if (_socket != null)
                {
                    await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "NormalClosure", CancellationToken.None);
                }
            }
            _socket?.Dispose();
            _socket = null;
        }
        IsConnected = false;
    }
}