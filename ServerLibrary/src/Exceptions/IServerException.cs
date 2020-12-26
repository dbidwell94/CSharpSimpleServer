using System;

namespace SimpleServer.Exceptions
{
    public interface IServerException
    {
        string Message { get; }
    }
}