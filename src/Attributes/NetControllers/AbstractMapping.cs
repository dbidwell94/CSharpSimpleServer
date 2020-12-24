using System;
using System.Net;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        public Regex PathRegex { get; private set; }

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
            var pathParser = new Regex(@":([^\/\\]+)");
            string regexString = "";
            regexString = Regex.Replace(path, @"(/)", @"\/");
            regexString = pathParser.Replace(regexString, @".*\/?");
            PathRegex = new Regex(regexString);
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

        public override string ToString()
        {
            return $"Path: {Path} -- Produces: {Produces} -- Accepts: {Accepts}";
        }


    }

    /// <summary>
    /// Contains information about the Method Controller
    /// </summary>
    public struct MappingInfo<T> where T : IAbstractMapping
    {
        public T Mapping { get; private set; }
        public MethodInfo Method { get; private set; }
        public Dictionary<string, PathParamInfo> RequiredParams { get; private set; }

        public Type ClassContainer { get; private set; }
        public MappingInfo(T mapping, MethodInfo method, Type classContainer, Dictionary<string, PathParamInfo> pathParam)
        {
            Mapping = mapping;
            Method = method;
            ClassContainer = classContainer;
            RequiredParams = pathParam;

        }

        public override string ToString()
        {
            string paramList = "";
            foreach (var param in RequiredParams)
            {
                paramList += $"  Param: {param} \n";
            }

            return $" Mapping: {Mapping} \n Method: {Method.Name} \n Params:\n {paramList} \n Container: {ClassContainer}\n\n";
        }
    }
}