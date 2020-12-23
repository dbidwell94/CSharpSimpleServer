using System;
using System.Net;
using SimpleServer.Attributes;
using SimpleServer.Exceptions;
using SimpleServer.Networking.Data;
using System.Threading.Tasks;
using System.Reflection;

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
                switch (httpMethod)
                {
                    case "GET":
                        GetResponse(GetMapping.FindPath(path, HttpMethod.GET, context), context);
                        break;

                    case "POST":
                        GetResponse(PostMapping.FindPath(path, HttpMethod.POST, context), context);
                        break;

                    case "DELETE":
                        GetResponse(DeleteMapping.FindPath(path, HttpMethod.DELETE, context), context);
                        break;

                    case "PUT":
                        GetResponse(PutMapping.FindPath(path, HttpMethod.PUT, context), context);
                        break;

                    case "PATCH":
                        GetResponse(PatchMapping.FindPath(path, HttpMethod.PATCH, context), context);
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

        }

        private static async void GetResponse<T>(MappingInfo<T> mappingInfo, HttpListenerContext context) where T : IAbstractMapping
        {
            await Task.Run(() =>
            {
                try
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = mappingInfo.mapping.Produces;
                    var controller = mappingInfo.classContainer.GetConstructor(new Type[] { }).Invoke(new object[] { });
                    var result = mappingInfo.method.Invoke(controller, new object[] { });
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
            });
        }
    }
}