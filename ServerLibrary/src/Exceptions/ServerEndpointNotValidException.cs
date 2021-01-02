using System;
using System.Net;
using SimpleServer.Networking.Data;

namespace SimpleServer.Exceptions
{
    public class ServerEndpointNotValidException : AbstractServerException
    {

        public ServerEndpointNotValidException(string message, HttpListenerContext currentContext) : base(message)
        {
            currentContext.Response.StatusCode = Status.HasValue ? (int)Status.Value : (int)HttpStatus.NOT_FOUND;
            currentContext.Response.ContentType = MediaTypes.ApplicationJson;
        }

        public ServerEndpointNotValidException(string message, HttpListenerContext currentContext, HttpStatus status) : base(message, status)
        {
            currentContext.Response.StatusCode = Status.HasValue ? (int)Status.Value : (int)HttpStatus.NOT_FOUND;
            currentContext.Response.ContentType = MediaTypes.ApplicationJson;
        }

        public ServerEndpointNotValidException(string message, HttpListenerContext currentContext, Exception innerException) : base(message, innerException)
        {
            currentContext.Response.StatusCode = Status.HasValue ? (int)Status.Value : (int)HttpStatus.NOT_FOUND;
            currentContext.Response.ContentType = MediaTypes.ApplicationJson;
        }

        public ServerEndpointNotValidException(string message, HttpListenerContext currentContext, Exception innerException, HttpStatus status) : base(message, innerException, status)
        {
            currentContext.Response.StatusCode = Status.HasValue ? (int)Status.Value : (int)HttpStatus.NOT_FOUND;
            currentContext.Response.ContentType = MediaTypes.ApplicationJson;
        }
    }
}