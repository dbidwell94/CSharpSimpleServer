using System;

namespace SimpleServer.Attributes
{
    /// <summary>
    /// A dynamic parameter extracted from the url string
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class PathParam : Attribute
    {
        public PathParam()
        {

        }
    }
}