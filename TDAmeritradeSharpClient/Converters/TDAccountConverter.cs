using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

/// <summary>
///     We must use a custom account converter because TDA has sub-types on TDAccount, and System.Text.Json doesn't handle inheritance on deserialization
/// </summary>
public class TDAccountConverter : JsonConverter<SecuritiesAccount>
{
    private readonly Stack<object> _currentObjects = new();

    /// <summary>
    ///     The object we are currently deserializing
    /// </summary>
    private object CurrentObject => _currentObjects.Peek();

    /// <summary>
    ///     The type of the _currentObject
    /// </summary>
    private Type CurrentType => CurrentObject.GetType();

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(SecuritiesAccount).IsAssignableFrom(typeToConvert);
    }

    public override SecuritiesAccount Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
        var accountTypeStr = reader.GetString();
        if (string.IsNullOrEmpty(accountTypeStr))
        {
            throw new JsonException();
        }
        Enum.TryParse<AccountType>(accountTypeStr, out var accountType);
        SecuritiesAccount account = accountType switch
        {
            AccountType.CASH => new CashAccount(),
            AccountType.MARGIN => new MarginAccount(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        //DebugAllReads(ref reader);

        var (properties, propertyNamesDictByCamelCaseNames) = StartObject(account);
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.None:
                    break;
                case JsonTokenType.StartObject:
                    // New properties on this new object
                    if (CurrentType == typeof(List<Position>))
                    {
                        var position = new Position();
                        account.Positions.Add(position);
                        (properties, propertyNamesDictByCamelCaseNames) = StartObject(position);
                    }
                    else if (CurrentType == typeof(List<TDOrder>))
                    {
                        var orderStrategy = new TDOrder();
                        account.OrderStrategies.Add(orderStrategy);
                        (properties, propertyNamesDictByCamelCaseNames) = StartObject(orderStrategy);
                    }
                    else if (CurrentType == typeof(List<OrderLeg>))
                    {
                        var orderLeg = new OrderLeg();
                        var order = (TDOrder)_currentObjects.ElementAt(_currentObjects.Count - 3);
                        order.OrderLegCollection.Add(orderLeg);
                        (properties, propertyNamesDictByCamelCaseNames) = StartObject(orderLeg);
                    }
                    else
                    {
                        throw new NotSupportedException(CurrentObject.ToString());
                    }
                    break;
                case JsonTokenType.EndObject:
                    if (CurrentObject == account)
                    {
                        return account;
                    }
                    (properties, propertyNamesDictByCamelCaseNames) = EndObject();
                    break;
                case JsonTokenType.StartArray:
                    throw new JsonException("Unexpected to get here.");
                case JsonTokenType.EndArray:
                    // Pop the list from _currentObjects
                    (properties, propertyNamesDictByCamelCaseNames) = EndObject();
                    break;
                case JsonTokenType.PropertyName:
                    ReadProperty(ref reader, ref propertyNamesDictByCamelCaseNames, ref properties, account);
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
        return account;
    }

    private void ReadProperty(ref Utf8JsonReader reader, ref Dictionary<string, string> propertyNamesDictByCamelCaseNames, ref PropertyDescriptorCollection properties,
        SecuritiesAccount account)
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
            throw new JsonException($"Unexpected missing property in {CurrentObject}: {propertyName}");
        }
        reader.Read();
        switch (reader.TokenType)
        {
            case JsonTokenType.None:
                throw new JsonException("Unexpected to get here.");
            case JsonTokenType.StartObject:
                object obj;
                switch (propertyName)
                {
                    case "initialBalances":
                        obj = account.Type == AccountType.CASH ? ((CashAccount)account).InitialBalances : ((MarginAccount)account).InitialBalances;
                        break;
                    case "currentBalances":
                        obj = account.Type == AccountType.CASH ? ((CashAccount)account).CurrentBalances : ((MarginAccount)account).CurrentBalances;
                        break;
                    case "projectedBalances":
                        obj = account.Type == AccountType.CASH ? ((CashAccount)account).ProjectedBalances : ((MarginAccount)account).ProjectedBalances;
                        break;
                    default:
                        throw new NotSupportedException(propertyName);
                }
                (properties, propertyNamesDictByCamelCaseNames) = StartObject(obj);
                break;
            case JsonTokenType.EndObject:
                (properties, propertyNamesDictByCamelCaseNames) = EndObject();
                break;
            case JsonTokenType.StartArray:
                // Must start a list of the type describe by propertyName
                if (propertyName == JsonNamingPolicy.CamelCase.ConvertName(nameof(SecuritiesAccount.Positions)))
                {
                    // Push this onto the stack. Will be immediately followed by a StartObject for Position
                    StartObject(account.Positions);
                }
                else if (propertyName == JsonNamingPolicy.CamelCase.ConvertName(nameof(SecuritiesAccount.OrderStrategies)))
                {
                    // Push this onto the stack. Will be immediately followed by a StartObject for TDOrder
                    StartObject(account.OrderStrategies);
                }
                else if (propertyName == JsonNamingPolicy.CamelCase.ConvertName(nameof(TDOrder.OrderLegCollection)))
                {
                    // Push this onto the stack. Will be immediately followed by a StartObject for OrderLeg
                    var tmp = _currentObjects.Peek();
                    var order = (TDOrder)CurrentObject;
                    StartObject(order.OrderLegCollection);
                }
                else
                {
                    throw new NotSupportedException(propertyName);
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
                    throw new JsonException($"Unexpected null string in {CurrentObject}: {propertyName}");
                }
                if (property.PropertyType.IsEnum)
                {
                    var enumValue = Enum.Parse(property.PropertyType, str);
                    property.SetValue(CurrentObject, enumValue);
                }
                else
                {
                    property.SetValue(CurrentObject, str);
                }
                break;
            case JsonTokenType.Number:
                object num;
                switch (property.PropertyType.ToString())
                {
                    case "System.Int32":
                        num = reader.GetInt32();
                        break;
                    case "System.Int64":
                        num = reader.GetInt64();
                        break;
                    case "System.Double":
                    case "System.Nullable`1[System.Double]":
                        num = reader.GetDouble();
                        break;
                    default:
                        throw new NotSupportedException();
                }
                property.SetValue(CurrentObject, num);
                break;
            case JsonTokenType.True:
                property.SetValue(CurrentObject, true);
                break;
            case JsonTokenType.False:
                property.SetValue(CurrentObject, false);
                break;
            case JsonTokenType.Null:
                // We get here at the end of an instrument when deserializing as a member of some class
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private (PropertyDescriptorCollection properties, Dictionary<string, string> propertyNamesDictByCamelCaseNames) StartObject(object obj)
    {
        _currentObjects.Push(obj);
        var (properties, propertyNamesDictByCamelCaseNames) = obj.SetPropertiesInfoForType();
        return (properties, propertyNamesDictByCamelCaseNames);
    }

    private (PropertyDescriptorCollection properties, Dictionary<string, string> propertyNamesDictByCamelCaseNames) EndObject()
    {
        _currentObjects.Pop();
        var (properties, propertyNamesDictByCamelCaseNames) = CurrentObject.SetPropertiesInfoForType();
        return (properties, propertyNamesDictByCamelCaseNames);
    }

    private static void DebugAllReads(ref Utf8JsonReader reader)
    {
        var received = new List<(JsonTokenType, object?)>();
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.None:
                    received.Add((reader.TokenType, null));
                    break;
                case JsonTokenType.StartObject:
                    received.Add((reader.TokenType, null));
                    break;
                case JsonTokenType.EndObject:
                    received.Add((reader.TokenType, null));
                    break;
                case JsonTokenType.StartArray:
                    received.Add((reader.TokenType, null));
                    break;
                case JsonTokenType.EndArray:
                    received.Add((reader.TokenType, null));
                    break;
                case JsonTokenType.PropertyName:
                    var propName = reader.GetString();
                    received.Add((reader.TokenType, propName));
                    break;
                case JsonTokenType.Comment:
                    received.Add((reader.TokenType, null));
                    break;
                case JsonTokenType.String:
                    var str = reader.GetString();
                    received.Add((reader.TokenType, str));
                    break;
                case JsonTokenType.Number:
                    var num = reader.GetDecimal();
                    received.Add((reader.TokenType, num));
                    break;
                case JsonTokenType.True:
                    received.Add((reader.TokenType, null));
                    break;
                case JsonTokenType.False:
                    received.Add((reader.TokenType, null));
                    break;
                case JsonTokenType.Null:
                    received.Add((reader.TokenType, null));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        var _ = received;
    }

    public override void Write(Utf8JsonWriter writer, SecuritiesAccount value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Use Client.SerializeAccount() which passes the derived type for polymorphic serialization");
    }
}