using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
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
            return await Task.Run(async () =>
            {
                List<object> methodParams = new List<object>();
                foreach (var param in mappingInfo.Method.GetParameters())
                {
                    if (param.GetCustomAttribute<PathParam>() != null)
                    {
                        var contextPath = context.Request.Url.AbsolutePath;
                        System.Console.WriteLine(contextPath);
                    }
                }

                return methodParams.ToArray();
            });
        }
    }
}