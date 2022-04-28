using System.Runtime.Serialization;

namespace TDAmeritradeSharpClient
{
    [Serializable]
    public class TDAmeritradeSharpException : Exception
    {
        public TDAmeritradeSharpException()
        {
        }

        public TDAmeritradeSharpException(string message)
            : base(message)
        {
        }

        public TDAmeritradeSharpException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Ensure Exception is Serializable
        protected TDAmeritradeSharpException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
        }
    }
}