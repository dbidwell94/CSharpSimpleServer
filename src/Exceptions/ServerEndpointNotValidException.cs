using System.Net;

namespace SimpleServer.Exceptions
{
    public class ServerEndpointNotValidException : AbstractServerException
    {

        public ServerEndpointNotValidException(string message, HttpListenerContext currentContext) : base(message)
        {
            currentContext.Response.StatusCode = 404;
            currentContext.Response.ContentType = "application/json";
        }
    }
}