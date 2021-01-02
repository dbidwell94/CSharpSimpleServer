using System;
using SimpleServer.Networking.Data;

namespace SimpleServer.Exceptions
{
    public abstract class AbstractServerException : Exception, IServerException
    {
        public override string Message { get; }

        public HttpStatus? Status {get; private set;}

        public AbstractServerException(string message) : base(message)
        {
            Message = message;
        }

        public AbstractServerException(string message, Exception innerException) : base(message, innerException)
        {
            Message = message;
        }

        public AbstractServerException(string message, HttpStatus status) : base(message)
        {
            Message = message;
            Status = status;
        }

        public AbstractServerException(string message, Exception innerException, HttpStatus status) : base(message, innerException)
        {
            Message = message;
            Status = status;
        }
    }
}