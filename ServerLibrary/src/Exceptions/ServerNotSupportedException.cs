using System;

namespace SimpleServer.Exceptions
{
    public class ServerNotSupportedException : AbstractServerException
    {
        public ServerNotSupportedException(string message) : base(message)
        { 
        }
    
        public ServerNotSupportedException(string message, Exception innerException) : base(message, innerException)
        { 
        }
    }
}