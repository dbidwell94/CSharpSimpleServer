using System.Net.Http.Headers;
using SimpleServer.Networking.Headers;

namespace SimpleServer.Networking
{
    public class ServerResponseHeaders : HttpHeaders
    {
        [HeaderName("Content-Range")]
        public string Range { get; private set; }

        [HeaderName("Access-Control-Allow-Origin")]
        public string Cors { get; private set; }

        [HeaderName("Location")]
        public string Location { get; set; }

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