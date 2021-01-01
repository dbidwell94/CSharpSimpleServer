using System;

namespace SimpleServer.Exceptions
{
    public class ServerMappingException : AbstractServerException
    {
        public ServerMappingException(string message) : base(message)
        {
        }
    
        public ServerMappingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}