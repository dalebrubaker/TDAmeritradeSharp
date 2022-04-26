using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TDAmeritradeSharpClient;

/// <summary>
///     Utility for deserializing stream messages
/// </summary>
public class TDStreamJsonProcessor
{
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

    public void Parse(string json)
    {
        var node = JsonNode.Parse(json);
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
        if (nodeData == null)
        {
            throw new InvalidOperationException();
        }
        var dataArray = nodeData.AsArray();
        Debug.Assert(dataArray != null, nameof(dataArray) + " != null");
        if (dataArray == null)
        {
            throw new InvalidOperationException();
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
                throw new InvalidOperationException();
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

    private void ParseHeartbeat(long timestamp)
    {
        var model = new TDHeartbeatSignal { timestamp = timestamp };
        OnHeartbeatSignal(model);
    }

    private void ParseBook(long timestamp, JsonObject content, string service)
    {
        var model = new TDBookSignal
        {
            timestamp = timestamp,
            id = (TDBookOptions)Enum.Parse(typeof(TDBookOptions), service)
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
                    model.symbol = item.Value.GetValue<string>();
                    break;
                //case "1":
                //    model.booktime = item.Value.Value<long>();
                //    break;
                case "2":
                    var arrayBids = item.Value.AsArray();
                    model.bids = arrayBids.Deserialize<TDBookLevel[]>()!;
                    break;
                case "3":
                    var arrayAsks = item.Value.AsArray();
                    model.asks = arrayAsks.Deserialize<TDBookLevel[]>()!;
                    break;
            }
        }
        OnBookSignal(model);
    }
    
    private void ParseChartFutures(long timestamp, JsonObject content)
    {
        var model = new TDChartSignal
        {
            timestamp = timestamp
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
                    model.symbol = item.Value.GetValue<string>();
                    break;
                case "seq":
                    model.sequence = item.Value.GetValue<long>();
                    break;
                case "1":
                    model.charttime = item.Value.GetValue<long>();
                    break;
                case "2":
                    model.openprice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model.highprice = item.Value.GetValue<double>();
                    break;
                case "4":
                    model.lowprice = item.Value.GetValue<double>();
                    break;
                case "5":
                    model.closeprice = item.Value.GetValue<double>();
                    break;
                case "6":
                    model.volume = item.Value.GetValue<double>();
                    break;
            }
        }
        OnChartSignal(model);
    }
    
    private void ParseChartEquity(long timestamp, JsonObject content)
    {
        var model = new TDChartSignal
        {
            timestamp = timestamp
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
                    model.symbol = item.Value.GetValue<string>();
                    break;
                case "seq":
                    model.sequence = item.Value.GetValue<long>();
                    break;
                case "1":
                    model.openprice = item.Value.GetValue<double>();
                    break;
                case "2":
                    model.highprice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model.lowprice = item.Value.GetValue<double>();
                    break;
                case "4":
                    model.closeprice = item.Value.GetValue<double>();
                    break;
                case "5":
                    model.volume = item.Value.GetValue<double>();
                    break;
                case "6":
                    model.sequence = item.Value.GetValue<long>();
                    break;
                case "7":
                    model.charttime = item.Value.GetValue<long>();
                    break;
                case "8":
                    model.chartday = item.Value.GetValue<int>();
                    break;
            }
        }
        OnChartSignal(model);
    }
    
    private void ParseTimeSaleEquity(long tmstamp, JsonObject content)
    {
        var model = new TDTimeSaleSignal
        {
            timestamp = tmstamp
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
                    model.symbol = item.Value.GetValue<string>();
                    break;
                case "seq":
                    model.sequence = item.Value.GetValue<long>();
                    break;
                case "1":
                    model.tradetime = item.Value.GetValue<long>();
                    break;
                case "2":
                    model.lastprice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model.lastsize = item.Value.GetValue<double>();
                    break;
                case "4":
                    model.lastsequence = item.Value.GetValue<long>();
                    break;
            }
        }
        OnTimeSaleSignal(model);
    }
    
    private void ParseQuote(long timestamp, JsonObject content)
    {
        var model = new TDQuoteSignal
        {
            timestamp = timestamp
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
                    model.symbol = item.Value.GetValue<string>();
                    break;
                case "1":
                    model.bidprice = item.Value.GetValue<double>();
                    break;
                case "2":
                    model.askprice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model.lastprice = item.Value.GetValue<double>();
                    break;
                case "4":
                    model.bidsize = item.Value.GetValue<double>();
                    break;
                case "5":
                    model.asksize = item.Value.GetValue<double>();
                    break;
                case "6":
                    model.askid = item.Value.GetValue<char>();
                    break;
                case "7":
                    model.bidid = item.Value.GetValue<char>();
                    break;
                case "8":
                    model.totalvolume = item.Value.GetValue<long>();
                    break;
                case "9":
                    model.lastsize = item.Value.GetValue<double>();
                    break;
                case "10":
                    model.tradetime = item.Value.GetValue<long>();
                    break;
                case "11":
                    model.quotetime = item.Value.GetValue<long>();
                    break;
                case "12":
                    model.highprice = item.Value.GetValue<double>();
                    break;
                case "13":
                    model.lowprice = item.Value.GetValue<double>();
                    break;
                case "14":
                    model.bidtick = item.Value.GetValue<char>();
                    break;
                case "15":
                    model.closeprice = item.Value.GetValue<double>();
                    break;
                case "24":
                    model.volatility = item.Value.GetValue<double>();
                    break;
                case "28":
                    model.openprice = item.Value.GetValue<double>();
                    break;
            }
        }
        OnQuoteSignal(model);
    }
}