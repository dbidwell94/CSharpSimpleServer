using System;
using System.Net;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleServer.Exceptions;
using SimpleServer.Networking;
using SimpleServer.Networking.Data;

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
            regexString = pathParser.Replace(regexString, @"(.*)\/?");
            regexString = regexString.Insert(0, "^");
            regexString = regexString.Insert(regexString.Length, "$");
            PathRegex = new Regex(regexString);
            Path = path;
        }

#nullable enable
        public static MappingInfo<AbstractMapping> FindPath(string path, HttpMethod method, HttpListenerContext? currentContext)
        {
            foreach (var map in Mapping[method].Keys)
            {
                if (Mapping[method][map].Mapping.PathRegex.IsMatch(path))
                {
                    if (currentContext != null)
                    {
                        currentContext.Response.ContentType = Mapping[method][map].Mapping.Produces;
                    }
                    return Mapping[method][map];
                }
            }
            throw new ServerEndpointNotValidException($"{method} {path} not a valid endpoint", currentContext, HttpStatus.NOT_FOUND);
        }
        public static MappingInfo<AbstractMapping> FindPath(HttpMethod method, string path, HttpListenerContext? currentContext)
        {
            return FindPath(path, method, currentContext);
        }
#nullable disable

        public static ResponseEntity HandleOptionsRequest(string path, HttpMethod method, string requestedHeaders)
        {
            string[] headerArr = requestedHeaders.Split(',');
            foreach (var map in Mapping[(HttpMethod)method].Keys)
            {
                if (Mapping[(HttpMethod)method][map].Mapping.PathRegex.IsMatch(path))
                {
                    if (Mapping[(HttpMethod)method][map].Method.GetCustomAttribute<AllowHeadersAttribute>() != null)
                    {
                        var allowedAttr = Mapping[(HttpMethod)method][map].Method.GetCustomAttribute<AllowHeadersAttribute>();
                        var headers = new ServerResponseHeaders();
                        headers.SetCors(SimpleServer.Networking.Headers.CorsHeader.BuildHeader("*"));
                        headers.AllowHeaders = allowedAttr.AllowedHeaders;
                        return new ResponseEntity(null, headers);
                    }
                    else
                    {
                        var headers = new ServerResponseHeaders();
                        headers.SetCors(SimpleServer.Networking.Headers.CorsHeader.BuildHeader("*"));
                        return new ResponseEntity(null, headers);
                    }

                }
            }
            return new ResponseEntity();
        }

        public override string ToString()
        {
            return $"Path: {Path} -- Produces: {Produces} -- Accepts: {Accepts}";
        }


    }

    /// <summary>
    /// Contains information about the Method Controller
    /// </summary>

#nullable enable
    public struct MappingInfo<T> where T : IAbstractMapping
    {
        public T Mapping { get; private set; }
        public MethodInfo Method { get; private set; }
        public Dictionary<string, PathParamInfo> RequiredParams { get; private set; }
        public RequestBodyInfo? RequiredRequestBody { get; private set; }

        public Type ClassContainer { get; private set; }
        public MappingInfo(T mapping, MethodInfo method, Type classContainer, Dictionary<string, PathParamInfo> pathParam, RequestBodyInfo? requestBody)
        {
            Mapping = mapping;
            Method = method;
            ClassContainer = classContainer;
            RequiredParams = pathParam;
            RequiredRequestBody = requestBody;
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
#nullable disable
}