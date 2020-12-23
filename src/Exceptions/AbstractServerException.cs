using System;

namespace SimpleServer.Exceptions
{
    public abstract class AbstractServerException : Exception, IServerException
    {
        public override string Message { get; }

        public AbstractServerException(string message) : base(message)
        {
            Message = message;
        }
    }
}