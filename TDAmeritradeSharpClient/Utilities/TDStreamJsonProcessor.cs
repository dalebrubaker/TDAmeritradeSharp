using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Serialization;
using Serilog;

namespace TDAmeritradeSharpClient;

/// <summary>
///     Utility for deserializing stream messages
/// </summary>
public class TDStreamJsonProcessor
{
    private static readonly ILogger s_logger = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType!);

    private readonly ClientStream _clientStream;

    public TDStreamJsonProcessor(ClientStream clientStream)
    {
        _clientStream = clientStream;
    }

    private JsonSerializerOptions JsonOptions => _clientStream.JsonOptions;

    /// <summary>
    ///     Parse the json received in a TDRealtimeResponseContainer and throw events on ClientStream with the result
    /// </summary>
    /// <param name="json"></param>
    /// <exception cref="TDAmeritradeSharpException"></exception>
    public void Parse(string json)
    {
        var node = JsonNode.Parse(json);
        var nodeResponse = node?["response"];
        if (nodeResponse != null)
        {
            var response = JsonSerializer.Deserialize<TDRealtimeResponseContainer>(json, JsonOptions);
            _clientStream.OnResponse(response ?? throw new TDAmeritradeSharpException());
            return;
        }
        var nodeNotify = node?["notify"];
        if (nodeNotify != null)
        {
            // Process heartbeat
            var array = nodeNotify!.AsArray();
            var nodeTimestamp = array[0]?["heartbeat"];
            if (nodeTimestamp != null)
            {
                var timestampStr = nodeTimestamp.GetValue<string>();
                long.TryParse(timestampStr, out var timestamp);
                ParseHeartbeat(timestamp);
                return;
            }
        }
        var nodeData = node?["data"];
        if (nodeData != null)
        {
            var dataArray = nodeData.AsArray();
            Debug.Assert(dataArray != null, nameof(dataArray) + " != null");
            if (dataArray == null)
            {
                throw new TDAmeritradeSharpException();
            }
            foreach (var arrayNode in dataArray)
            {
                if (arrayNode == null)
                {
                    continue;
                }
                var service = arrayNode["service"]?.GetValue<string>();
                var nodeTimestamp = arrayNode["timestamp"];
                if (nodeTimestamp == null)
                {
                    throw new TDAmeritradeSharpException();
                }
                var timestamp = nodeTimestamp.GetValue<long>();
                var contents = arrayNode["content"];
                if (contents == null)
                {
                    return;
                }
                var contentsArray = contents.AsArray();
                foreach (var jsonNode in contentsArray) //.Children<JObject>())
                {
                    if (jsonNode == null)
                    {
                        continue;
                    }
                    var content = (JsonObject)jsonNode;
                    switch (service)
                    {
                        case "ACCT_ACTIVITY":
                            ParseAcctActivity(timestamp, content);
                            break;
                        case "QUOTE":
                            ParseQuote(timestamp, content);
                            break;
                        case "CHART_FUTURES":
                            ParseChartFutures(timestamp, content);
                            break;
                        case "CHART_EQUITY":
                            ParseChartEquity(timestamp, content);
                            break;
                        case "LISTED_BOOK":
                        case "NASDAQ_BOOK":
                        case "OPTIONS_BOOK":
                            ParseBook(timestamp, content, service);
                            break;
                        case "TIMESALE_EQUITY":
                        case "TIMESALE_FUTURES":
                        case "TIMESALE_FOREX":
                        case "TIMESALE_OPTIONS":
                            ParseTimeSaleEquity(timestamp, content);
                            break;
                    }
                }
            }
        }
    }

    private void ParseHeartbeat(long timestamp)
    {
        var model = new TDHeartbeatSignal { Timestamp = timestamp };
        _clientStream.OnHeartbeatSignal(model);
    }

    private void ParseBook(long timestamp, JsonObject content, string service)
    {
        var model = new TDBookSignal
        {
            Timestamp = timestamp,
            Id = (TDBookOptions)Enum.Parse(typeof(TDBookOptions), service)
        };
        foreach (var item in content)
        {
            if (item.Value == null)
            {
                continue;
            }
            switch (item.Key)
            {
                case "key":
                    model.Symbol = item.Value.GetValue<string>();
                    break;
                //case "1":
                //    model.booktime = item.Value.Value<long>();
                //    break;
                case "2":
                    var arrayBids = item.Value.AsArray();
                    model.Bids = arrayBids.Deserialize<TDBookLevel[]>()!;
                    break;
                case "3":
                    var arrayAsks = item.Value.AsArray();
                    model.Asks = arrayAsks.Deserialize<TDBookLevel[]>()!;
                    break;
            }
        }
        _clientStream.OnBookSignal(model);
    }

    private void ParseChartFutures(long timestamp, JsonObject content)
    {
        var model = new TDChartSignal
        {
            Timestamp = timestamp
        };
        foreach (var item in content)
        {
            if (item.Value == null)
            {
                continue;
            }
            switch (item.Key)
            {
                case "key":
                    model.Symbol = item.Value.GetValue<string>();
                    break;
                case "seq":
                    model.Sequence = item.Value.GetValue<long>();
                    break;
                case "1":
                    model.ChartTime = item.Value.GetValue<long>();
                    break;
                case "2":
                    model.OpenPrice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model.HighPrice = item.Value.GetValue<double>();
                    break;
                case "4":
                    model.LowPrice = item.Value.GetValue<double>();
                    break;
                case "5":
                    model.ClosePrice = item.Value.GetValue<double>();
                    break;
                case "6":
                    model.Volume = item.Value.GetValue<double>();
                    break;
            }
        }
        _clientStream.OnChartSignal(model);
    }

    private void ParseChartEquity(long timestamp, JsonObject content)
    {
        var model = new TDChartSignal
        {
            Timestamp = timestamp
        };
        foreach (var item in content)
        {
            if (item.Value == null)
            {
                continue;
            }
            switch (item.Key)
            {
                case "key":
                    model.Symbol = item.Value.GetValue<string>();
                    break;
                case "seq":
                    model.Sequence = item.Value.GetValue<long>();
                    break;
                case "1":
                    model.OpenPrice = item.Value.GetValue<double>();
                    break;
                case "2":
                    model.HighPrice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model.LowPrice = item.Value.GetValue<double>();
                    break;
                case "4":
                    model.ClosePrice = item.Value.GetValue<double>();
                    break;
                case "5":
                    model.Volume = item.Value.GetValue<double>();
                    break;
                case "6":
                    model.Sequence = item.Value.GetValue<long>();
                    break;
                case "7":
                    model.ChartTime = item.Value.GetValue<long>();
                    break;
                case "8":
                    model.ChartDay = item.Value.GetValue<int>();
                    break;
            }
        }
        _clientStream.OnChartSignal(model);
    }

    private void ParseTimeSaleEquity(long tmstamp, JsonObject content)
    {
        var model = new TDTimeSaleSignal
        {
            Timestamp = tmstamp
        };
        foreach (var item in content)
        {
            if (item.Value == null)
            {
                continue;
            }
            switch (item.Key)
            {
                case "key":
                    model.Symbol = item.Value.GetValue<string>();
                    break;
                case "seq":
                    model.Sequence = item.Value.GetValue<long>();
                    break;
                case "1":
                    model.TradeTime = item.Value.GetValue<long>();
                    break;
                case "2":
                    model.LastPrice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model.LastSize = item.Value.GetValue<double>();
                    break;
                case "4":
                    model.LastSequence = item.Value.GetValue<long>();
                    break;
            }
        }
        _clientStream.OnTimeSaleSignal(model);
    }

    private void ParseQuote(long timestamp, JsonObject content)
    {
        var model = new TDQuoteSignal
        {
            Timestamp = timestamp
        };
        foreach (var item in content)
        {
            if (item.Value == null)
            {
                continue;
            }
            switch (item.Key)
            {
                case "key":
                    model.Symbol = item.Value.GetValue<string>();
                    break;
                case "1":
                    model.BidPrice = item.Value.GetValue<double>();
                    break;
                case "2":
                    model.AskPrice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model.LastPrice = item.Value.GetValue<double>();
                    break;
                case "4":
                    model.BidSize = item.Value.GetValue<double>();
                    break;
                case "5":
                    model.AskSize = item.Value.GetValue<double>();
                    break;
                case "6":
                    model.AskId = item.Value.GetValue<char>();
                    break;
                case "7":
                    model.BidId = item.Value.GetValue<char>();
                    break;
                case "8":
                    model.TotalVolume = item.Value.GetValue<long>();
                    break;
                case "9":
                    model.LastSize = item.Value.GetValue<double>();
                    break;
                case "10":
                    model.TradeTime = item.Value.GetValue<long>();
                    break;
                case "11":
                    model.QuoteTime = item.Value.GetValue<long>();
                    break;
                case "12":
                    model.HighPrice = item.Value.GetValue<double>();
                    break;
                case "13":
                    model.LowPrice = item.Value.GetValue<double>();
                    break;
                case "14":
                    model.BidTick = item.Value.GetValue<char>();
                    break;
                case "15":
                    model.ClosePrice = item.Value.GetValue<double>();
                    break;
                case "24":
                    model.Volatility = item.Value.GetValue<double>();
                    break;
                case "28":
                    model.OpenPrice = item.Value.GetValue<double>();
                    break;
            }
        }
        _clientStream.OnQuoteSignal(model);
    }

    /// <summary>
    ///     See https://developer.tdameritrade.com/content/streaming-data#_Toc504640580 for documentatin
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="content"></param>
    /// <exception cref="TDAmeritradeSharpException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    private void ParseAcctActivity(long timestamp, JsonObject content)
    {
        //var keys = content.ToList().Select(x => x.Key).ToList();
        var node2 = content["2"];
        var messageType = node2?.GetValue<string>();
        var nodeXml = content["3"];
        var messageData = nodeXml?.GetValue<string>();
        using var reader = new StringReader(messageData ?? throw new TDAmeritradeSharpException());
        try
        {
            switch (messageType)
            {
                case "SUBSCRIBED":
                    return;
                case "ERROR":
                    throw new NotImplementedException(messageData);
                case "BrokenTrade":
                    throw new NotImplementedException(messageData);
                case "ManualExecution":
                    throw new NotImplementedException(messageData);
                case "OrderActivation":
                    throw new NotImplementedException(messageData);
                case "OrderCancelReplaceRequest":
                    {
                        var serializer = new XmlSerializer(typeof(OrderCancelReplaceRequestMessage));
                        var request = (OrderCancelReplaceRequestMessage)serializer.Deserialize(reader);
                        _clientStream.OnOrderCancelReplaceRequestMessage(request);
                    }
                    break;
                case "OrderCancelRequest":
                    {
                        var serializer = new XmlSerializer(typeof(OrderCancelRequestMessage));
                        var request = (OrderCancelRequestMessage)serializer.Deserialize(reader);
                        _clientStream.OnOrderCancelRequest(request);
                    }
                    break;
                case "OrderEntryRequest":
                    {
                        var serializer = new XmlSerializer(typeof(OrderEntryRequestMessage));
                        var request = (OrderEntryRequestMessage)serializer.Deserialize(reader);
                        _clientStream.OnOrderEntryRequest(request);
                    }
                    break;
                case "OrderFill":
                    throw new NotImplementedException(messageData);
                case "OrderPartialFill":
                    throw new NotImplementedException(messageData);
                case "OrderRejection":
                    throw new NotImplementedException(messageData);
                case "TooLateToCancel":
                    throw new NotImplementedException(messageData);
                case "UROUT":
                    {
                        var serializer = new XmlSerializer(typeof(UROUTMessage));
                        var uroutMessage = (UROUTMessage)serializer.Deserialize(reader);
                        _clientStream.OnUROUTMessage(uroutMessage);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            s_logger.Error(ex, "{Message}", ex.Message);
            throw;
        }
    }
}