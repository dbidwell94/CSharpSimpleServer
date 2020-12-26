using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using SimpleServer.Attributes;
using SimpleServer.Exceptions;
using SimpleServer.Networking.Data;
using Newtonsoft.Json;
namespace SimpleServer.Networking
{
    internal static class ContextRunner
    {
        public delegate void ContextEvent(ServerEventData eventData);
        public static event ContextEvent onRequestFinishedProcessing;


        public static void RunWith(HttpListenerContext context)
        {
            string path = context.Request.Url.PathAndQuery;
            string httpMethod = context.Request.HttpMethod;
            try
            {
                HttpMethod method = (HttpMethod)Enum.Parse(typeof(HttpMethod), httpMethod, true);
                switch (method)
                {
                    case HttpMethod.GET:
                        GetResponse(GetMapping.FindPath(path, method, context), context);
                        break;

                    case HttpMethod.POST:
                        GetResponse(PostMapping.FindPath(path, method, context), context);
                        break;

                    case HttpMethod.DELETE:
                        GetResponse(DeleteMapping.FindPath(path, method, context), context);
                        break;

                    case HttpMethod.PUT:
                        GetResponse(PutMapping.FindPath(path, method, context), context);
                        break;

                    case HttpMethod.PATCH:
                        GetResponse(PatchMapping.FindPath(path, method, context), context);
                        break;

                    default:
                        throw new ServerRequestMethodNotSupportedException($"{httpMethod} is not supported", context);
                }
            }
            catch (ServerRequestMethodNotSupportedException ex)
            {
                ExceptionHandler.HandleException(ex, context);
            }
            catch (ServerEndpointNotValidException ex)
            {
                ExceptionHandler.HandleException(ex, context);
            }
            catch (Exception ex)
            {
                try
                {
                    throw new InternalServerErrorException(ex.Message, context);
                }
                catch (InternalServerErrorException e)
                {
                    ExceptionHandler.HandleException(e, context);
                }
            }

        }

        private static async void GetResponse<T>(MappingInfo<T> mappingInfo, HttpListenerContext context) where T : IAbstractMapping
        {
            await Task.Run(async () =>
            {
                try
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = mappingInfo.Mapping.Produces;
                    var controller = mappingInfo.ClassContainer.GetConstructor(new Type[] { }).Invoke(new object[] { });
                    object result;
                    if (mappingInfo.Method.GetParameters().Length > 0)
                    {
                        var param = await ParseParams(mappingInfo, context);
                        result = mappingInfo.Method.Invoke(controller, param);
                    }
                    else
                    {
                        result = mappingInfo.Method.Invoke(controller, new object[] { });
                    }
                    if (result.GetType() == typeof(ResponseEntity))
                    {
                        var resultResponse = (ResponseEntity)result;
                        SendResponse(resultResponse, context);
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        throw new InternalServerErrorException(e.Message, context);
                    }
                    catch (InternalServerErrorException ex)
                    {
                        ExceptionHandler.HandleException(ex, context);
                    }
                }
            });
        }

        public static async void SendResponse(ResponseEntity data, HttpListenerContext currentContext)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var byteBuffer = data.GetDataAsBytes();
                    currentContext.Response.ContentLength64 = byteBuffer.Length;
                    await currentContext.Response.OutputStream.WriteAsync(byteBuffer, 0, byteBuffer.Length);
                    currentContext.Response.OutputStream.Close();
                    var eventData = new ServerEventData
                    {
                        status = currentContext.Response.StatusCode,
                        exception = null,
                        message = null,
                        path = currentContext.Request.Url.AbsolutePath
                    };
                    onRequestFinishedProcessing?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    try
                    {
                        throw new InternalServerErrorException(e.Message, currentContext);
                    }
                    catch (InternalServerErrorException ex)
                    {
                        ExceptionHandler.HandleException(ex, currentContext);
                    }
                }
            });
        }

        public static async Task<object[]> ParseParams<T>(MappingInfo<T> mappingInfo, HttpListenerContext context) where T : IAbstractMapping
        {
            string path = context.Request.Url.AbsolutePath;
            return await Task.Run(async () =>
            {
                object[] methodParams = new object[mappingInfo.Method.GetParameters().Length];
                var pathParamMatches = mappingInfo.Mapping.PathRegex.Match(path);
                if (pathParamMatches.Groups.Count > 1)
                {
                    for (int i = 1; i < pathParamMatches.Groups.Count; i++)
                    {
                        int pathParamIndex = i - 1;
                        var m = pathParamMatches.Groups[i];
                        foreach (var paramInfo in mappingInfo.RequiredParams.Values)
                        {
                            if (paramInfo.ParamPathIndex == pathParamIndex)
                            {
                                var converted = Convert.ChangeType(m.Value, paramInfo.ParamType);
                                methodParams[paramInfo.ParamMethodIndex] = converted;
                            }
                        }
                    }
                }
                if (mappingInfo.RequiredRequestBody != null)
                {
                    object result;
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        var resultString = await reader.ReadToEndAsync();
                        var settings = new JsonSerializerSettings();
                        settings.NullValueHandling = NullValueHandling.Ignore;
                        settings.Formatting = Formatting.None;
                        settings.MaxDepth = 15;
                        result = JsonConvert.DeserializeObject(resultString, mappingInfo.RequiredRequestBody.Value.ParamType, settings);
                    }
                    methodParams[mappingInfo.RequiredRequestBody.Value.ParamMethodIndex] = result;
                }
                return methodParams;
            });
        }
    }
}