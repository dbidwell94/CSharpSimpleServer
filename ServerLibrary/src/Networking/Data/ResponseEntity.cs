using System.Text;
using Newtonsoft.Json;

namespace SimpleServer.Networking.Data
{
    public class ResponseEntity : IJsonSerializeable
    {
        public static JsonSerializerSettings serializerSettings { get; private set; }
        public object Data { get; private set; } = null;

        public ResponseEntity()
        {

        }

        static ResponseEntity()
        {
            serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public ResponseEntity(object data)
        {
            Data = data;
        }

        public string JSON
        {
            get { return JsonConvert.SerializeObject(Data, serializerSettings); }
        }

        public byte[] GetDataAsBytes()
        {
            byte[] buffer = new byte[] { };
            buffer = Encoding.ASCII.GetBytes(JSON);
            return buffer;
        }
    }
}