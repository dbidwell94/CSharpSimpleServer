using System.Runtime.CompilerServices;
using System;

namespace SimpleServer.Networking.Headers
{
    public sealed class CorsHeader
    {
        internal string Path { get; private set; }

        public static CorsHeader BuildHeader(string allowedUri)
        {
            var toReturn = new CorsHeader();
            if (allowedUri != "*" && Uri.TryCreate(allowedUri, UriKind.RelativeOrAbsolute, out Uri result))
            {
                toReturn.Path = result.Host;
                if (result.Port != 80)
                {
                    toReturn.Path += $":{result.Port}";
                }
            }
            else
            {
                toReturn.Path = allowedUri;
            }
            return toReturn;
        }
    }
}