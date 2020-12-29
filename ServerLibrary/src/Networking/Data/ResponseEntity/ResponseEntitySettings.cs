using System;
using Newtonsoft.Json;

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

        public ResponseEntitySettings()
        {
            JsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

    }
}