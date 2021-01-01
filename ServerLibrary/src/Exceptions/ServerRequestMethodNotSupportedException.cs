using System;
using System.Net;

namespace SimpleServer.Exceptions
{
    public class ServerRequestMethodNotSupportedException : AbstractServerException
    {
        public ServerRequestMethodNotSupportedException(string message, HttpListenerContext currentContext) : base(message)
        {
            currentContext.Response.StatusCode = 500;
        }
    
        public ServerRequestMethodNotSupportedException(string message, HttpListenerContext currentContext, Exception innerException) : base(message, innerException)
        {
            currentContext.Response.StatusCode = 500;
        }
    }
}