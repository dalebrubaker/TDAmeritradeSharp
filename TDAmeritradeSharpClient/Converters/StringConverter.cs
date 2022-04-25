using System.Text.Json;
using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

public class StringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    {
                        var stringValue = reader.GetInt32();
                        return stringValue.ToString();
                    }
                case JsonTokenType.String:
                    return reader.GetString() ?? throw new InvalidOperationException();
                default:
                    throw new JsonException();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}