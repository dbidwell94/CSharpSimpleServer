using System;

namespace SimpleServer.Attributes
{
    
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowHeadersAttribute : Attribute
    {
        public string AllowedHeaders { get; set; }
        public string[] HeaderArr { get; private set; }

        /// <summary>
        /// Decorate a controller with this attribute to handle Options requests for various headers
        /// </summary>
        /// <param name="allowedHeaders">a string containing allowed headers seperated by commas</param>
        public AllowHeadersAttribute(string allowedHeaders)
        {
            AllowedHeaders = allowedHeaders;
            HeaderArr = AllowedHeaders.Split(',');
        }
    }
}