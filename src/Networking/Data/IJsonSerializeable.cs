using System;

namespace SimpleServer.Networking.Data
{
    public interface IJsonSerializeable
    {
        string ParseToJson();
    }
}