using System.Net.Http.Headers;
using SimpleServer.Networking.Headers;

namespace SimpleServer.Networking
{
    public class ServerResponseHeaders : HttpHeaders
    {
        public string ContentType { get; set; }

        public string Range { get; private set; }

        public string Cors{ get; private set; }

        public void SetRange(RangeHeader header)
        {
            Range = header.Range;
        }

        public void SetCors(CorsHeader header)
        {
            Cors = header.Path;
        }
    }
}