using System;
using System.Reflection;
using System.Collections.Generic;

namespace SimpleServer.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public static Dictionary<string, Object> Services { get; private set; }
        public string Name { get; set; }

        static ServiceAttribute()
        {
            Services = new Dictionary<string, object>();
        }

        public ServiceAttribute(string name)
        {
            Name = name;
        }

        internal static void RegisterServices()
        {
            foreach (var t in Assembly.GetEntryAssembly().GetTypes())
            {
                if (t.GetCustomAttribute<ServiceAttribute>() != null)
                {
                    var instanciatedObject = t.GetConstructor(new Type[] { }).Invoke(new object[] { });
                    var serviceInfo = t.GetCustomAttribute<ServiceAttribute>();
                    Services.Add(serviceInfo.Name, instanciatedObject);
                }
            }
        }
    }
}