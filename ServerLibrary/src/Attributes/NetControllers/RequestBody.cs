using System;

namespace SimpleServer.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RequestBody : Attribute
    {

    }

    public struct RequestBodyInfo
    {
        public Type ParamType { get; private set; }
        public int ParamMethodIndex { get; private set; }

        public RequestBodyInfo(Type type, int methodIndex)
        {
            ParamType = type;
            ParamMethodIndex = methodIndex;
        }

        public override string ToString()
        {
            return $"{ParamType} -- methodIndex:{ParamMethodIndex}";
        }
    }
}