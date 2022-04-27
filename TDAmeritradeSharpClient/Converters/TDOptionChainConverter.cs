using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

public class TDOptionChainConverter : JsonConverter<TDOptionChain>
{
    public override TDOptionChain? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);
        if (node == null)
        {
            throw new InvalidOperationException();
        }
        var result = new TDOptionChain
        {
            symbol = node["symbol"]?.GetValue<string>(),
            status = node["status"]?.GetValue<string>(),
            underlying = node["underlying"].Deserialize<TDUnderlying>(),
            strategy = node["strategy"]?.GetValue<string>(),
            interval = node["interval"]!.GetValue<double>(),
            isDelayed = node["isDelayed"]!.GetValue<bool>(),
            isIndex = node["isIndex"]!.GetValue<bool>(),
            daysToExpiration = node["daysToExpiration"]!.GetValue<double>(),
            interestRate = node["interestRate"]!.GetValue<double>(),
            underlyingPrice = node["underlyingPrice"]!.GetValue<double>(),
            volatility = node["volatility"]!.GetValue<double>(),
            callExpDateMap = GetMap(node["callExpDateMap"]),
            putExpDateMap = GetMap(node["putExpDateMap"])
        };
        return result;
    }

    private TDOptionMap GetMap(JsonNode? node)
    {
        var exp = new TDOptionMap();
        var jsonObject = (JsonObject)node!;
        foreach (KeyValuePair<string,JsonNode?> pair in jsonObject)
        {
            exp.expires = DateTime.Parse(pair.Key.Split(':')[0]);
            exp.optionsAtStrike = new List<TDOptionsAtStrike>(); // This is an array in TDA json although we seem to have only one instance in each array
            var optionsAtStrike = new TDOptionsAtStrike();
            exp.optionsAtStrike.Add(optionsAtStrike);
            Debug.Assert(jsonObject.Count > 0);
            var strikeObject = (JsonObject)pair.Value!;
            var strikeNode = strikeObject.FirstOrDefault();
            double.TryParse(strikeNode.Key, out var strikePrice);
            optionsAtStrike.strikePrice = strikePrice;
            optionsAtStrike.options = new List<TDOption>();
            var array = strikeNode.Value?.AsArray();
            foreach (var optionNode in array!)
            {
                var option = optionNode.Deserialize<TDOption>();
                optionsAtStrike.options.Add(option ?? throw new InvalidOperationException());
            }
        }
        return exp;
    }

    public override void Write(Utf8JsonWriter writer, TDOptionChain value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}