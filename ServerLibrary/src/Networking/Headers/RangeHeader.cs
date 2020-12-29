using System.Collections.Generic;

namespace SimpleServer.Networking.Headers
{
    public sealed class RangeHeader
    {
        internal List<RangeHeaderValue> RangeHeaders { get; private set; } = new List<RangeHeaderValue>();
        internal string Range
        {
            get
            {
                if (RangeHeaders.Count == 0)
                {
                    return null;
                }
                string builtString = "bytes=";

                for (int i = 0; i < RangeHeaders.Count; i++)
                {
                    string subString = "";

                    if (RangeHeaders[i].LowerRange.HasValue)
                    {
                        subString += RangeHeaders[i].LowerRange.Value.ToString();
                    }
                    subString += "-";
                    if (RangeHeaders[i].UpperRange.HasValue)
                    {
                        subString += RangeHeaders[i].UpperRange.Value.ToString();
                    }

                    if (i < RangeHeaders.Count - 1)
                    {
                        subString += ", ";
                    }
                    builtString += subString;
                }

                return builtString;
            }
        }
        public void AddRange(RangeHeaderValue headerValue)
        {
            if (headerValue.Validate())
            {
                RangeHeaders.Add(headerValue);
            }
        }

        /// <summary>
        /// Create an instance of RangeHeader using RangeHeaderValues
        /// </summary>
        /// <param name="values">A collection of RangeHeaderValues that represent upper and lower byte ranges</param>
        /// <returns></returns>
        public static RangeHeader Create(IEnumerable<RangeHeaderValue> values)
        {
            var toReturn = new RangeHeader();
            foreach (var value in values)
            {
                if (value.Validate())
                {
                    toReturn.AddRange(value);
                }
            }
            return toReturn;
        }
    }
    /// <summary>
    /// A easy way to set the Range header. UpperRange and LowerRange are in bytes
    /// </summary>
    public struct RangeHeaderValue
    {
        public long? UpperRange { get; private set; }
        public long? LowerRange { get; private set; }

        /// <summary>
        /// Create a new RangeHeaderValue with the specified Uppoer and Lower range
        /// </summary>
        /// <param name="lowerRange">The return value's starting point in bytes</param>
        /// <param name="upperRange">The return value's ending point in bytes</param>
        public RangeHeaderValue(long? lowerRange, long? upperRange)
        {
            UpperRange = upperRange;
            LowerRange = lowerRange;
        }

        public bool Validate()
        {
            if (!UpperRange.HasValue && !LowerRange.HasValue)
            {
                return false;
            }
            return true;
        }
    }
}