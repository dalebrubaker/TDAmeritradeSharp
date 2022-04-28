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
            throw new TDAmeritradeSharpException();
        }
        var result = new TDOptionChain
        {
            Symbol = node["symbol"]?.GetValue<string>(),
            Status = node["status"]?.GetValue<string>(),
            Underlying = node["underlying"].Deserialize<TDUnderlying>(),
            Strategy = node["strategy"]?.GetValue<string>(),
            Interval = node["interval"]!.GetValue<double>(),
            IsDelayed = node["isDelayed"]!.GetValue<bool>(),
            IsIndex = node["isIndex"]!.GetValue<bool>(),
            DaysToExpiration = node["daysToExpiration"]!.GetValue<double>(),
            InterestRate = node["interestRate"]!.GetValue<double>(),
            UnderlyingPrice = node["underlyingPrice"]!.GetValue<double>(),
            Volatility = node["volatility"]!.GetValue<double>(),
            CallExpDateMap = GetMap(node["callExpDateMap"]),
            PutExpDateMap = GetMap(node["putExpDateMap"])
        };
        return result;
    }

    private TDOptionMap GetMap(JsonNode? node)
    {
        var exp = new TDOptionMap();
        var jsonObject = (JsonObject)node!;
        foreach (KeyValuePair<string,JsonNode?> pair in jsonObject)
        {
            exp.Expires = DateTime.Parse(pair.Key.Split(':')[0]);
            exp.OptionsAtStrike = new List<TDOptionsAtStrike>(); // This is an array in TDA json although we seem to have only one instance in each array
            var optionsAtStrike = new TDOptionsAtStrike();
            exp.OptionsAtStrike.Add(optionsAtStrike);
            Debug.Assert(jsonObject.Count > 0);
            var strikeObject = (JsonObject)pair.Value!;
            var strikeNode = strikeObject.FirstOrDefault();
            double.TryParse(strikeNode.Key, out var strikePrice);
            optionsAtStrike.StrikePrice = strikePrice;
            optionsAtStrike.Options = new List<TDOption>();
            var array = strikeNode.Value?.AsArray();
            foreach (var optionNode in array!)
            {
                var option = optionNode.Deserialize<TDOption>();
                optionsAtStrike.Options.Add(option ?? throw new TDAmeritradeSharpException());
            }
        }
        return exp;
    }

    public override void Write(Utf8JsonWriter writer, TDOptionChain value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}