using System;
using System.Net;
using System.Reflection;
using System.Collections.Generic;
using SimpleServer.Exceptions;
using SimpleServer.Networking;

namespace SimpleServer.Attributes
{
    /// <summary>
    /// Abstract template for Get,Post,etc. mappings
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class AbstractMapping : Attribute, IAbstractMapping
    {

        /// <summary>
        /// Main string: HTTP Method, value string: Path
        /// </summary>
        /// <value></value>
        public static Dictionary<HttpMethod, Dictionary<string, MappingInfo<AbstractMapping>>> Mapping { get; private set; } =
            new Dictionary<HttpMethod, Dictionary<string, MappingInfo<AbstractMapping>>>();

        public string Path { get; set; }

        public string Accepts { get; set; }

        public string Produces { get; set; }

        static AbstractMapping()
        {
            foreach (var item in Enum.GetValues(typeof(HttpMethod)))
            {
                HttpMethod castItem = (HttpMethod)item;
                Mapping.Add(castItem, new Dictionary<string, MappingInfo<AbstractMapping>>());
            }
        }

        public AbstractMapping(string path)
        {
            Path = path;
        }

        public static MappingInfo<AbstractMapping> FindPath(string path, HttpMethod method, HttpListenerContext currentContext)
        {
            if (!Mapping[method].ContainsKey(path))
            {
                throw new ServerEndpointNotValidException($"{method} {path} not a valid endpoint", currentContext);
            }
            return Mapping[method][path];
        }
        public static MappingInfo<AbstractMapping> FindPath(HttpMethod method, string path, HttpListenerContext currentContext)
        {
            if (!Mapping[method].ContainsKey(path))
            {
                throw new ServerEndpointNotValidException($"{method} {path} not a valid endpoint", currentContext);
            }
            return Mapping[method][path];
        }

    }

    /// <summary>
    /// Contains information about the Method Controller
    /// </summary>
    public struct MappingInfo<T> where T : IAbstractMapping
    {
        public T mapping;
        public MethodInfo method;
        public Type classContainer;
        public MappingInfo(T mapping, MethodInfo method, Type classContainer)
        {
            this.mapping = mapping;
            this.method = method;
            this.classContainer = classContainer;
        }
    }
}