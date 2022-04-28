using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

public class TDAccountConverter : JsonConverter<TDPrincipalAccount>
{
    public override TDPrincipalAccount? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, TDPrincipalAccount value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Use Client.SerializeAccount() which passes the derived type for polymorphic serialization");
    }
}