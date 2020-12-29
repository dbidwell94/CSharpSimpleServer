using System;

namespace SimpleServer.Networking.Headers
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class HeaderNameAttribute : Attribute
    {
        public string HttpName{ get; set; }
        public HeaderNameAttribute(string headerName)
        {
            HttpName = headerName;
        }
    }
}