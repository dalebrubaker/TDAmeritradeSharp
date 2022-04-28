using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

/// <summary>
/// We must use a custom instrument converter because TDA has sub-types on Instrument, and System.Text.Json doesn't handle inheritance on deserialization
/// </summary>
public class TDInstrumentConverter : JsonConverter<TDInstrument>
{
    /// <summary>
    ///     The object we are currently deserializing
    /// </summary>
    private object? _currentObject;

    /// <summary>
    ///     The type of the _currentObject
    /// </summary>
    private Type? _currentType;

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(TDInstrument).IsAssignableFrom(typeToConvert);
    }

    public override TDInstrument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }
        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }
        reader.Read();
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }
        var assetTypeStr = reader.GetString();
        if (string.IsNullOrEmpty(assetTypeStr))
        {
            throw new JsonException();
        }
        Enum.TryParse<TDOrderEnums.AssetType>(assetTypeStr, out var assetType);
        TDInstrument instrument = assetType switch
        {
            TDOrderEnums.AssetType.EQUITY => new InstrumentEquity(),
            TDOrderEnums.AssetType.OPTION => new InstrumentOption(),
            TDOrderEnums.AssetType.INDEX => new InstrumentIndex(),
            TDOrderEnums.AssetType.MUTUAL_FUND => new InstrumentMutualFund(),
            TDOrderEnums.AssetType.CASH_EQUIVALENT => new InstrumentCashEquivalent(),
            TDOrderEnums.AssetType.FIXED_INCOME => new InstrumentFixedIncome(),
            TDOrderEnums.AssetType.CURRENCY => new InstrumentCurrency(),
            _ => throw new ArgumentOutOfRangeException()
        };
        var (properties, propertyNamesDictByCamelCaseNames) = instrument.SetPropertiesInfoForType();
        _currentObject = instrument;
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.None:
                    break;
                case JsonTokenType.StartObject:
                    // New properties on this new object
                    _currentObject = Activator.CreateInstance(_currentType ?? throw new TDAmeritradeSharpException());
                    if (_currentType == typeof(OptionDeliverable))
                    {
                        var instrumentOption = (InstrumentOption)instrument;
                        instrumentOption.OptionDeliverables!.Add((OptionDeliverable)_currentObject);
                    }
                    (properties, propertyNamesDictByCamelCaseNames) = _currentObject.SetPropertiesInfoForType();
                    break;
                case JsonTokenType.EndObject:
                    if (_currentObject == instrument)
                    {
                        return instrument;
                    }
                    // We may be at the end of a sub-object 
                    _currentObject = instrument;
                    (properties, propertyNamesDictByCamelCaseNames) = instrument.SetPropertiesInfoForType();
                    break;
                case JsonTokenType.StartArray:
                    throw new JsonException("Unexpected to get here.");
                case JsonTokenType.EndArray:
                    break;
                case JsonTokenType.PropertyName:
                    ReadProperty(ref reader, ref propertyNamesDictByCamelCaseNames, ref properties, instrument);
                    break;
                case JsonTokenType.Comment:
                    throw new JsonException("Unexpected to get here.");
                case JsonTokenType.String:
                    throw new JsonException("Unexpected to get here.");
                case JsonTokenType.Number:
                    throw new JsonException("Unexpected to get here.");
                case JsonTokenType.True:
                    throw new JsonException("Unexpected to get here.");
                case JsonTokenType.False:
                    throw new JsonException("Unexpected to get here.");
                case JsonTokenType.Null:
                    throw new JsonException("Unexpected to get here.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return instrument;
    }

    private void ReadProperty(ref Utf8JsonReader reader, ref Dictionary<string, string> propertyNamesDictByCamelCaseNames, ref PropertyDescriptorCollection properties, TDInstrument instrument)
    {
        var propertyName = reader.GetString();
        if (propertyName == null)
        {
            throw new TDAmeritradeSharpException();
        }
        var propName = propertyNamesDictByCamelCaseNames[propertyName];
        var property = properties[propName];
        if (property == null)
        {
            throw new JsonException($"Unexpected missing property in {_currentObject}: {propertyName}");
        }
        reader.Read();
        switch (reader.TokenType)
        {
            case JsonTokenType.None:
                throw new JsonException("Unexpected to get here.");
            case JsonTokenType.StartObject:
                _currentObject = Activator.CreateInstance(_currentType ?? throw new TDAmeritradeSharpException());
                (properties, propertyNamesDictByCamelCaseNames) = _currentObject.SetPropertiesInfoForType();
                break;
            case JsonTokenType.EndObject:
                // Assume we are going back to setting the instrument
                // Do NOT change _currentType because we may be doing a series of objects in an array
                _currentObject = instrument;
                (properties, propertyNamesDictByCamelCaseNames) = instrument.SetPropertiesInfoForType();
                break;
            case JsonTokenType.StartArray:
                // Must start a list of the type describe by propertyName
                if (propertyName == JsonNamingPolicy.CamelCase.ConvertName(nameof(InstrumentOption.OptionDeliverables)))
                {
                    var instrumentOption = (InstrumentOption)instrument;
                    instrumentOption.OptionDeliverables = new List<OptionDeliverable>();
                    _currentType = typeof(OptionDeliverable);
                }
                break;
            case JsonTokenType.EndArray:
                throw new JsonException("Unexpected to get here.");
            case JsonTokenType.PropertyName:
                throw new JsonException("Unexpected to get here.");
            case JsonTokenType.Comment:
                throw new JsonException("Unexpected to get here.");
            case JsonTokenType.String:
                var str = reader.GetString();
                if (str == null)
                {
                    throw new JsonException($"Unexpected null string in {_currentObject}: {propertyName}");
                }
                if (property.PropertyType.IsEnum)
                {
                    var enumValue = Enum.Parse(property.PropertyType, str);
                    property.SetValue(_currentObject, enumValue);
                }
                else
                {
                    property.SetValue(_currentObject, str);
                }
                break;
            case JsonTokenType.Number:
                var num = reader.GetDouble();
                property.SetValue(_currentObject, num);
                break;
            case JsonTokenType.True:
                throw new JsonException("Unexpected to get here.");
            case JsonTokenType.False:
                throw new JsonException("Unexpected to get here.");
            case JsonTokenType.Null:
                // We get here at the end of an instrument when deserializing as a member of some class
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void Write(Utf8JsonWriter writer, TDInstrument value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Use Client.SerializeInstrument() which passes the derived type for polymorphic serialization");
    }
}