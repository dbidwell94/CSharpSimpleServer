using System;

namespace SimpleServer.Attributes
{
    /// <summary>
    /// This attribute should be placed above a method within a class decorated
    /// with <see cref="SimpleServer.Attributes.ConfigAttribute" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExceptionHandlerAttribute : Attribute
    {
        public Type Handles { get; set; }

        /// <summary>
        /// Decorates this method telling SimpleServer to handle exceptions of this type
        /// </summary>
        /// <param name="handles">The typeof() Type to handle. Should be a SimpleServer Exception type from 
        /// <see cref="SimpleServer.Exceptions" /></param>
        public ExceptionHandlerAttribute(Type handles)
        {
            Handles = handles;
        }
    }
}