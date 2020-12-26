using System;

namespace SimpleServer.Exceptions
{
    public class ServerNotSupportedException : AbstractServerException
    {
        public ServerNotSupportedException(string message) : base(message)
        {
            
        }
    }
}