using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SimpleServer.Networking.Data
{
    public class ResponseEntitySettings
    {
        internal JsonSerializerSettings JsonSerializerSettings { get; private set; } = new JsonSerializerSettings();
        public bool SendNullValues
        {
            get
            {
                if (JsonSerializerSettings.NullValueHandling == NullValueHandling.Ignore)
                {
                    return false;
                }
                else return true;
            }
            set
            {
                if (value)
                {
                    JsonSerializerSettings.NullValueHandling = NullValueHandling.Include;
                }
                else JsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }
        }

        public bool AcceptRanges { get; set; } = false;

        public ResponseEntitySettings()
        {
            JsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            JsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}