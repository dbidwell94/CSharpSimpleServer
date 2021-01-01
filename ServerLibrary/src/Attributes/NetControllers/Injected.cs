using System.Reflection;
using System.Net.Http.Headers;
using System.Net;
using System;
using System.Collections.Generic;

namespace SimpleServer.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class InjectedAttribute : Attribute
    {
        internal struct ParameterTypeInfo
        {
            public int ParameterPosition { get; set; }
            public Object ParameterObject { get; set; }
        }

        internal static ParameterTypeInfo[] FindParameters(MethodInfo toInject, HttpListenerContext currentContext)
        {
            var toReturn = new List<ParameterTypeInfo>();
            foreach (var param in toInject.GetParameters())
            {
                if (param.GetCustomAttribute<InjectedAttribute>() != null)
                {
                    if (param.ParameterType == typeof(WebHeaderCollection))
                    {
                        toReturn.Add(new ParameterTypeInfo
                        {
                            ParameterPosition = param.Position,
                            ParameterObject = currentContext.Request.Headers
                    });
                    }
                }
            }
            return toReturn.ToArray();
        }
    }
}