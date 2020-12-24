using System;

namespace SimpleServer.Attributes
{
    /// <summary>
    /// A dynamic parameter extracted from the url string. Method parameter must match case exactly as the url path specifies
    /// Example:
    /// [GetMapping("/route/:path")] => [PathParam] string path
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class PathParam : Attribute
    {
        public PathParam()
        {
        }
    }

    public struct PathParamInfo
    {
        public string ParamName { get; private set; }
        public Type ParamType { get; private set; }
        public int ParamMethodIndex { get; private set; }
        public int ParamPathIndex { get; private set; }

        public PathParamInfo(string name, Type type, int methodIndex, int pathIndex)
        {
            ParamName = name;
            ParamType = type;
            ParamMethodIndex = methodIndex;
            ParamPathIndex = pathIndex;
        }

        public override string ToString()
        {
            return $"{ParamName} -- {ParamType} -- methodIndex:{ParamMethodIndex} -- pathIndex:{ParamPathIndex}";
        }
    }
}