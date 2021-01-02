using System;
using System.Net;
using SimpleServer.Networking.Data;

namespace SimpleServer.Exceptions
{
    public class ServerRequestMethodNotSupportedException : AbstractServerException
    {
        public ServerRequestMethodNotSupportedException(string message, HttpListenerContext currentContext) : base(message)
        {
            currentContext.Response.StatusCode = Status.HasValue ? (int)Status.Value : (int)HttpStatus.INTERNAL_SERVER_ERROR;
        }
    
        public ServerRequestMethodNotSupportedException(string message, HttpListenerContext currentContext, Exception innerException) : base(message, innerException)
        {
            currentContext.Response.StatusCode = Status.HasValue ? (int)Status.Value : (int)HttpStatus.INTERNAL_SERVER_ERROR;
        }
    }
}