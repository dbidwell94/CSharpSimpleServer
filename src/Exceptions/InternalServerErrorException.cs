using System.Net;

namespace SimpleServer.Exceptions
{
    public class InternalServerErrorException : AbstractServerException
    {
        public InternalServerErrorException(string message, HttpListenerContext currentContext) : base(message)
        {
            currentContext.Response.StatusCode = 500;
        }
    }
}