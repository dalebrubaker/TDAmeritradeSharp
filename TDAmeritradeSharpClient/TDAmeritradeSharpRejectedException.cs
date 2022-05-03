using System.Runtime.Serialization;

namespace TDAmeritradeSharpClient;

[Serializable]
public class TDAmeritradeSharpRejectedException : Exception
{
    public TDAmeritradeSharpRejectedException()
    {
    }

    public TDAmeritradeSharpRejectedException(string message)
        : base(message)
    {
    }

    public TDAmeritradeSharpRejectedException(string message, Exception inner)
        : base(message, inner)
    {
    }

    // Ensure Exception is Serializable
    protected TDAmeritradeSharpRejectedException(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
    {
    }
}