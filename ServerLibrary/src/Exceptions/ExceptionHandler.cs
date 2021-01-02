using System;
using System.Reflection;
using SimpleServer.Attributes;
using SimpleServer.Networking.Data;
using SimpleServer.Networking;
using System.Net;

namespace SimpleServer.Exceptions
{
    public static class ExceptionHandler
    {
        #region Events
        public delegate void ExceptionHandlerDelegate(AbstractServerException exception);
        public static event ExceptionHandlerDelegate onError;
        #endregion

        private struct ExceptionHandlerInfo
        {
            public MethodInfo method;
            public Type callingType;
            public ExceptionHandlerInfo(MethodInfo method, Type callingType)
            {
                this.method = method;
                this.callingType = callingType;
            }
        }

        private static IServerExceptionHandler Handler = null;
        static ExceptionHandler()
        {
            foreach (var c in Assembly.GetEntryAssembly().GetTypes())
            {
                if (c.GetCustomAttribute<ConfigAttribute>() != null)
                {
                    var inter = c.GetInterface(typeof(IServerExceptionHandler).FullName);
                    if (inter != null)
                    {
                        Handler = c.GetConstructor(new Type[] { }).Invoke(new object[] { }) as IServerExceptionHandler;
                    }
                }
            }
        }
        /// <summary>
        /// Uses reflection to handle exceptions defined in a <see cref="SimpleServer.Attributes.ConfigAttribute" />
        /// decorator.
        /// </summary>
        /// <param name="exception">An instance of a thrown <see cref="SimpleServer.Exceptions.AbstractServerException" /></param>
        public static void HandleException(AbstractServerException exception, HttpListenerContext currentContext)
        {
            HandleException(exception, out ResponseEntity response);
            onError?.Invoke(exception);
            ContextRunner.SendResponse(response, currentContext);
        }

        public static void HandleException(AbstractServerException exception, out ResponseEntity response)
        {

            HttpStatus status = exception.Status.HasValue ? exception.Status.Value : HttpStatus.INTERNAL_SERVER_ERROR;

            if (Handler == null)
            {
                response = new ResponseEntity(exception, status);
            }
            else if (exception.GetType() == typeof(ServerEndpointNotValidException))
            {
                var ex = (ServerEndpointNotValidException)exception;
                response = Handler.HandleEndpointNotValidException(ex);
                response.Status = status;
            }
            else if (exception.GetType() == typeof(ServerRequestMethodNotSupportedException))
            {
                var ex = (ServerRequestMethodNotSupportedException)exception;
                response = Handler.HandleServerRequestMethodNotSupportedException(ex);
                response.Status = status;
            }
            else if (exception.GetType() == typeof(InternalServerErrorException))
            {
                var ex = (InternalServerErrorException)exception;
                response = Handler.HandleInternalServerErrorException(ex);
                response.Status = status;
            }
            else if (exception.GetType() == typeof(AbstractServerException))
            {
                response = Handler.HandleAbstractServerException(exception);
                response.Status = status;
            }
            else
            {
                if (exception.GetType().IsSubclassOf(typeof(AbstractServerException)))
                {
                    response = Handler.HandleAbstractServerException(exception);
                    response.Status = status;
                }
                else
                {
                    response = new ResponseEntity(exception.Message, status);
                }
            }
        }
    }
}