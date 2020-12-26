using System;

namespace SimpleServer.Attributes
{
    /// <summary>
    /// Decorate a class with this attribute to specify a configuration using method decorators
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : Attribute
    {
        
    }
}