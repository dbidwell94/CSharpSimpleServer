using System.Text;
using Newtonsoft.Json;

namespace SimpleServer.Networking.Data
{
    public class ResponseEntity : IJsonSerializeable
    {
        public ResponseEntitySettings ResponseEntitySettings { get; set; }
        public object Data { get; private set; } = null;

        public ResponseEntity()
        {
            ResponseEntitySettings = new ResponseEntitySettings();
        }

        public ResponseEntity(object data) : this()
        {
            this.Data = data;
        }

        public ResponseEntity(object data, ResponseEntitySettings settings)
        {
            Data = data;
            ResponseEntitySettings = settings;
        }

        public string JSON
        {
            get { return JsonConvert.SerializeObject(Data, ResponseEntitySettings.JsonSerializerSettings); }
        }

        public byte[] GetDataAsBytes()
        {
            byte[] buffer = new byte[] { };
            buffer = Encoding.ASCII.GetBytes(JSON);
            return buffer;
        }
    }
}