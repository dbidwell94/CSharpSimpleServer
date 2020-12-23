using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace SimpleServer.Networking.Data
{
    public class ResponseEntity : IJsonSerializeable
    {

        public object Data { get; private set; } = null;

        public ResponseEntity()
        {

        }

        public ResponseEntity(object data)
        {
            Data = data;
        }

        public string ParseToJson()
        {
            return JsonConvert.SerializeObject(Data);
        }

        public byte[] GetDataAsBytes()
        {
            byte[] buffer = new byte[] { };
            buffer = Encoding.ASCII.GetBytes(ParseToJson());
            return buffer;
        }
    }
}