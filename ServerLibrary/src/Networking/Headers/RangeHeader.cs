
namespace SimpleServer.Networking.Headers
{
    public sealed class RangeHeader
    {
        internal RangeHeaderValue? RawRange { get; private set; }
        internal string Range
        {
            get
            {
                if (RawRange == null)
                {
                    return null;
                }
                return $"bytes {RawRange.Value.LowerRange}-{RawRange.Value.UpperRange}/{RawRange.Value.UpperRange - RawRange.Value.LowerRange}";
            }
        }

        /// <summary>
        /// Create an instance of RangeHeader using RangeHeaderValues
        /// </summary>
        /// <param name="values">A collection of RangeHeaderValues that represent upper and lower byte ranges</param>
        /// <returns></returns>
        public static RangeHeader Create(RangeHeaderValue value)
        {
            var toReturn = new RangeHeader();
            toReturn.RawRange = value;
            return toReturn;
        }
    }
    /// <summary>
    /// A easy way to set the Range header. UpperRange and LowerRange are in bytes
    /// </summary>
    public struct RangeHeaderValue
    {
        public long UpperRange { get; private set; }
        public long LowerRange { get; private set; }

        /// <summary>
        /// Create a new RangeHeaderValue with the specified Uppoer and Lower range
        /// </summary>
        /// <param name="lowerRange">The return value's starting point in bytes</param>
        /// <param name="upperRange">The return value's ending point in bytes</param>
        public RangeHeaderValue(long lowerRange, long upperRange)
        {
            UpperRange = upperRange;
            LowerRange = lowerRange;
        }
    }
}