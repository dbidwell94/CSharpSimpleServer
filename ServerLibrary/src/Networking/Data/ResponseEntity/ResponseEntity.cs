using System;
using System.Text;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using SimpleServer.Networking.Headers;

namespace SimpleServer.Networking.Data
{
    public class ResponseEntity : IJsonSerializeable
    {
        public ResponseEntitySettings ResponseEntitySettings { get; set; }
        public ServerResponseHeaders Headers { get; private set; }
        public object Data { get; private set; } = null;
        public string JSON
        {
            get { return JsonConvert.SerializeObject(Data, ResponseEntitySettings.JsonSerializerSettings); }
        }

        public ResponseEntity()
        {
            if (this.Headers == null)
            {
                Headers = new ServerResponseHeaders();
            }
            if (ResponseEntitySettings == null)
            {
                ResponseEntitySettings = new ResponseEntitySettings();
            }
        }

        public ResponseEntity(object data) : this()
        {
            Data = data;
        }

        public ResponseEntity(object data, ServerResponseHeaders headers) : this(data)
        {
            Headers = headers;
        }

        public ResponseEntity(object data, ResponseEntitySettings settings) : this(data)
        {
            ResponseEntitySettings = settings;
        }

        public ResponseEntity(object data, ServerResponseHeaders headers, ResponseEntitySettings settings) : this(data, headers)
        {
            ResponseEntitySettings = settings;
        }

        public byte[] GetDataAsBytes()
        {
            byte[] buffer = new byte[] { };
            buffer = Encoding.ASCII.GetBytes(JSON);
            return buffer;
        }

        public void ParseHeadersIntoContext(HttpListenerContext currentContext)
        {
            foreach (var prop in Headers.GetType().GetProperties())
            {
                if (prop.GetCustomAttribute<HeaderNameAttribute>() != null)
                {
                    var propInfo = prop.GetCustomAttribute<HeaderNameAttribute>();
                    var propValue = (string)prop.GetValue(Headers);
                    Console.WriteLine($"{propInfo.HttpName} - {propValue}");
                    if (propValue != null && propValue != "")
                    {
                        currentContext.Response.Headers.Add(propInfo.HttpName, propValue);
                    }
                }
            }
            var programName = Assembly.GetCallingAssembly().GetName();
            currentContext.Response.Headers.Add("Server", $"{programName.Name}/{programName.Version}");
            if (ResponseEntitySettings.AcceptRanges)
            {
                currentContext.Response.Headers.Add("Accept-Ranges", "bytes");
            }
        }
    }
}