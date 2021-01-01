using System;
using System.Net;
using SimpleServer.Networking.Data;

namespace SimpleServer.Exceptions
{
    public class InternalServerErrorException : AbstractServerException
    {
        public InternalServerErrorException(string message, HttpListenerContext currentContext) : base(message)
        {
            currentContext.Response.StatusCode = 500;
            currentContext.Response.ContentType = MediaTypes.ApplicationJson;
        }

        public InternalServerErrorException(string message, HttpListenerContext currentContext, Exception innerException) : base(message, innerException)
        {
            currentContext.Response.StatusCode = 500;
            currentContext.Response.ContentType = MediaTypes.ApplicationJson;
        }
    }
}