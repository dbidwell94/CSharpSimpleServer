using System.Net;

namespace SimpleServer.Exceptions
{
    public class ServerRequestMethodNotSupportedException : AbstractServerException
    {
        public ServerRequestMethodNotSupportedException(string message, HttpListenerContext currentContext) : base(message)
        {
            currentContext.Response.StatusCode = 500;
        }
    }
}