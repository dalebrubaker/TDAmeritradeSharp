using System.Text.Json.Serialization;

namespace TDAmeritradeSharpClient;

[Serializable]
public struct TDPriceCandle
{
    public double Close { get; set; }
    public double Datetime { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Open { get; set; }
    public double Volume { get; set; }

    /// <summary>
    /// Timestamp of the START of the bar
    /// </summary>
    [JsonIgnore]
    public DateTime DateTime
    {
        get => TDHelpers.FromUnixTimeMilliseconds(Datetime);
        set => Datetime = value.ToUnixTimeSeconds();
    }

    public override string ToString()
    {
        return $"{DateTime:g}";
    }
}

// ReSharper disable InconsistentNaming
[Serializable]
///https://developer.tdameritrade.com/content/price-history-samples
public struct TDPriceHistoryRequest
{
    [Serializable]
    public enum PeriodTypes
    {
        day,
        month,
        year,
        ytd
    }

    [Serializable]
    public enum FrequencyTypeEnum
    {
        minute,
        daily,
        weekly,
        monthly
    }
    // ReSharper restore InconsistentNaming

    public string Symbol { get; set; }

    /// <summary>
    ///     The type of period to show. Valid values are day, month, year, or ytd (year to date). Default is day.
    /// </summary>
    public PeriodTypes? PeriodType { get; set; }

    /// <summary>
    ///     The number of periods to show.
    ///     Example: For a 2 day / 1 min chart, the values would be:
    ///     period: 2
    ///     periodType: day
    ///     frequency: 1
    ///     frequencyType: min
    ///     Valid periods by periodType(defaults marked with an asterisk) :
    ///     day: 1, 2, 3, 4, 5, 10*
    ///     month: 1*, 2, 3, 6
    ///     year: 1*, 2, 3, 5, 10, 15, 20
    ///     ytd: 1*
    /// </summary>
    public int Period { get; set; }

    /// <summary>
    ///     The type of frequency with which a new candle is formed.
    ///     Valid frequencyTypes by periodType(defaults marked with an asterisk):
    ///     day: minute*
    ///     month: daily, weekly*
    ///     year: daily, weekly, monthly*
    ///     ytd: daily, weekly*
    /// </summary>
    public FrequencyTypeEnum? FrequencyType { get; set; }

    /// <summary>
    ///     The number of the frequencyType to be included in each candle.
    ///     /// Valid frequencies by frequencyType (defaults marked with an asterisk):
    ///     minute: 1*, 5, 10, 15, 30
    ///     daily: 1*
    ///     weekly: 1*
    ///     monthly: 1*
    /// </summary>
    public int Frequency { get; set; }

    /// <summary>
    ///     End date as milliseconds since epoch. If startDate and endDate are provided, period should not be provided. Default is previous trading day.
    /// </summary>
    public double? EndDate { get; set; }

    /// <summary>
    ///     Start date as milliseconds since epoch. If startDate and endDate are provided, period should not be provided.
    /// </summary>
    public double? StartDate { get; set; }

    /// <summary>
    ///     true to return extended hours data, false for regular market hours only. Default is true
    /// </summary>
    public bool? NeedExtendedHoursData { get; set; }

    [JsonIgnore]
    public DateTime? EndDateTime
    {
        get => EndDate.HasValue ? TDHelpers.FromUnixTimeSeconds(EndDate.Value) : null;
        set => EndDate = value?.ToUnixTimeSeconds();
    }

    [JsonIgnore]
    public DateTime? StartDateTime
    {
        get => StartDate.HasValue ? TDHelpers.FromUnixTimeSeconds(StartDate.Value) : null;
        set => StartDate = value?.ToUnixTimeSeconds();
    }
}