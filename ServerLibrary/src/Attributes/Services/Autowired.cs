using System.ComponentModel;
using System;
using System.Reflection;

namespace SimpleServer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutowiredAttribute : Attribute
    {
        internal static void InjectServices(Object toInject)
        {
            foreach (var prop in toInject.GetType().GetProperties())
            {
                if (prop.GetCustomAttribute<AutowiredAttribute>() != null)
                {
                    toInject.GetType().GetProperty(prop.Name).SetValue(toInject, ServiceAttribute.Services[prop.Name]);
                }
            }
        }
    }
}