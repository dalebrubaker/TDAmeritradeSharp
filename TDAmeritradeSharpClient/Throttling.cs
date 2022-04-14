namespace TDAmeritradeSharpClient;

/// <summary>
///     Maintain a list of the Put/Post/Delete commands throttled by the TD Ameritrade API
///     Personal application will be throttled to 0-120 POST/PUT/DELETE Order requests per minute per account
///     based on the properties of the application you specified during the application registration process.
///     GET order requests will be unthrottled.
/// </summary>
public static class Throttling
{
    private static readonly List<DateTime> s_throttledRequestTimesUtc = new();

    /// <summary>
    ///     A list of the DateTime.UtcNow when each request was made, pruned to the last minute
    /// </summary>
    public static List<DateTime> ThrottledThrottledRequestTimesUtc
    {
        get
        {
            var firstAllowed = DateTime.UtcNow.AddMinutes(-1);
            for (var i = 0; i < s_throttledRequestTimesUtc.Count; i++)
            {
                var timestamp = s_throttledRequestTimesUtc[i];
                if (timestamp < firstAllowed)
                {
                    s_throttledRequestTimesUtc.RemoveAt(i--);
                }
            }
            return s_throttledRequestTimesUtc;
        }
    }

    /// <summary>
    ///     Use this extension on POST/PUT/DELETE requests.
    ///     Personal application will be throttled to 0-120 POST/PUT/DELETE Order requests per minute per account
    ///     based on the properties of the application you specified during the application registration process.
    ///     GET order requests will be unthrottled.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public static HttpClient Throttle(this HttpClient client)
    {
        s_throttledRequestTimesUtc.Add(DateTime.UtcNow);
        return client;
    }
}