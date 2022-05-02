using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TDAmeritradeSharpClient;

/// <summary>
///     Utility for deserializing stream messages
/// </summary>
public class TDStreamJsonProcessor
{
    // private static readonly ILogger s_logger = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType!);

    private readonly ClientStream _clientStream;

    public TDStreamJsonProcessor(ClientStream clientStream)
    {
        _clientStream = clientStream;
    }

    private JsonSerializerOptions JsonOptions => _clientStream.JsonOptions;

    public string? Parse(string json)
    {
        var node = JsonNode.Parse(json);
        var nodeResponse = node?["response"];
        if (nodeResponse != null)
        {
            var response = JsonSerializer.Deserialize<TDRealtimeResponseContainer>(json, JsonOptions);
            return response?.ToString();
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
                return "heartbeat";
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
                    return null;
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
                    return service;
                }
            }
        }
        return null;
    }

    private void ParseAcctActivity(long timestamp, JsonObject content)
    {
        var node2 = content["2"];
        var str = node2?.GetValue<string>();
        if (str == "SUBSCRIBED")
        {
            return;
        }
        switch (str)
        {
            case "SUBSCRIBED":
                return;
            case "OrderEntryRequest":
                var nodeXml = content["3"];
                var encoding = nodeXml?.GetValue<string>();
                break;
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
}