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
            throw new TDAmeritradeSharpException();
        }
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
        var model = new TDHeartbeatSignal { Timestamp = timestamp };
        OnHeartbeatSignal(model);
    }

    private void ParseBook(long timestamp, JsonObject content, string service)
    {
        var model = new TDBookSignal
        {
            Timestamp = timestamp,
            _id = (TDBookOptions)Enum.Parse(typeof(TDBookOptions), service)
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
                    model._bids = arrayBids.Deserialize<TDBookLevel[]>()!;
                    break;
                case "3":
                    var arrayAsks = item.Value.AsArray();
                    model._asks = arrayAsks.Deserialize<TDBookLevel[]>()!;
                    break;
            }
        }
        OnBookSignal(model);
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
                    model._sequence = item.Value.GetValue<long>();
                    break;
                case "1":
                    model._charttime = item.Value.GetValue<long>();
                    break;
                case "2":
                    model._openprice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model._highprice = item.Value.GetValue<double>();
                    break;
                case "4":
                    model._lowprice = item.Value.GetValue<double>();
                    break;
                case "5":
                    model._closeprice = item.Value.GetValue<double>();
                    break;
                case "6":
                    model._volume = item.Value.GetValue<double>();
                    break;
            }
        }
        OnChartSignal(model);
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
                    model._sequence = item.Value.GetValue<long>();
                    break;
                case "1":
                    model._openprice = item.Value.GetValue<double>();
                    break;
                case "2":
                    model._highprice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model._lowprice = item.Value.GetValue<double>();
                    break;
                case "4":
                    model._closeprice = item.Value.GetValue<double>();
                    break;
                case "5":
                    model._volume = item.Value.GetValue<double>();
                    break;
                case "6":
                    model._sequence = item.Value.GetValue<long>();
                    break;
                case "7":
                    model._charttime = item.Value.GetValue<long>();
                    break;
                case "8":
                    model._chartday = item.Value.GetValue<int>();
                    break;
            }
        }
        OnChartSignal(model);
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
                    model._sequence = item.Value.GetValue<long>();
                    break;
                case "1":
                    model._tradetime = item.Value.GetValue<long>();
                    break;
                case "2":
                    model._lastprice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model._lastsize = item.Value.GetValue<double>();
                    break;
                case "4":
                    model._lastsequence = item.Value.GetValue<long>();
                    break;
            }
        }
        OnTimeSaleSignal(model);
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
                    model._bidprice = item.Value.GetValue<double>();
                    break;
                case "2":
                    model._askprice = item.Value.GetValue<double>();
                    break;
                case "3":
                    model._lastprice = item.Value.GetValue<double>();
                    break;
                case "4":
                    model._bidsize = item.Value.GetValue<double>();
                    break;
                case "5":
                    model._asksize = item.Value.GetValue<double>();
                    break;
                case "6":
                    model._askid = item.Value.GetValue<char>();
                    break;
                case "7":
                    model._bidid = item.Value.GetValue<char>();
                    break;
                case "8":
                    model._totalvolume = item.Value.GetValue<long>();
                    break;
                case "9":
                    model._lastsize = item.Value.GetValue<double>();
                    break;
                case "10":
                    model._tradetime = item.Value.GetValue<long>();
                    break;
                case "11":
                    model._quotetime = item.Value.GetValue<long>();
                    break;
                case "12":
                    model._highprice = item.Value.GetValue<double>();
                    break;
                case "13":
                    model._lowprice = item.Value.GetValue<double>();
                    break;
                case "14":
                    model._bidtick = item.Value.GetValue<char>();
                    break;
                case "15":
                    model._closeprice = item.Value.GetValue<double>();
                    break;
                case "24":
                    model._volatility = item.Value.GetValue<double>();
                    break;
                case "28":
                    model._openprice = item.Value.GetValue<double>();
                    break;
            }
        }
        OnQuoteSignal(model);
    }
}