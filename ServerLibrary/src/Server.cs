using System;
using System.Net;
using System.Threading;
using System.Reflection;
using SimpleServer.Exceptions;
using SimpleServer.Attributes;
using SimpleServer.Networking;

namespace SimpleServer
{
    public static class Server
    {
        /// <summary>
        /// Main Excecution thread
        /// </summary>
        private static Thread serverThread;

        #region Properties
        public static bool IsRunning { get; private set; }
        public static bool UsingHttps { get; private set; }
        public static int Port { get; private set; }
        public static int HttpsPort { get; private set; }
        #endregion

        #region Server Events
        public delegate void ServerDelegate(ServerEventData eventData);
        public static event ServerDelegate onEndpointRegistrationFinished;
        public static event ServerDelegate onServerStart;
        public static event ServerDelegate onRequestReceived;
        public static event ServerDelegate onServerError;
        public static event ServerDelegate onServerStop;

        #endregion

        /// <summary>
        /// Start the Http(s) server on specified port number
        /// </summary>
        /// <param name="portNumber">What number should the server run on</param>
        /// <param name="useHttps">Should the server use https (default false)</param>
        public static void Start(int portNumber, bool useHttps = false)
        {
            Port = portNumber;
            HttpsPort = Port + 1;
            UsingHttps = useHttps;
            serverThread.Start();
        }

        /// <summary>
        /// Start the Http(s) server on specified port number
        /// </summary>
        /// <param name="portNumber">What number should the server run on</param>
        /// <param name="httpsPortNumber">What number should https listener listen to?</param>
        /// <param name="useHttps">Should the server use https (default true)</param>
        public static void Start(int portNumber, int httpsPortNumber, bool useHttps = true)
        {
            Port = portNumber;
            HttpsPort = httpsPortNumber;
            UsingHttps = useHttps;
            serverThread.Start();
        }

        static Server()
        {
            serverThread = new Thread(() => Run());
            ContextRunner.onRequestFinishedProcessing += (msg) => onRequestReceived?.Invoke(msg);
            ExceptionHandler.onError += (err) =>
            {
                var serverEvent = new ServerEventData(null, null, err, null);
                onServerError?.Invoke(serverEvent);
            };
        }

        private static void HaultServerThread(ServerEventData data)
        {
            onServerStop -= HaultServerThread;
            throw new Exception(data.message);
        }

        private static void Run()
        {
            if (HttpListener.IsSupported)
            {
                var listener = new HttpListener();
                listener.Start();
                listener.Prefixes.Add($"http://*:{Port}/");
                onServerStart?.Invoke(new ServerEventData(null, null, null, $"Server started on port {Port}"));
                if (UsingHttps)
                {
                    listener.Prefixes.Add($"https://*:{HttpsPort}");
                    onServerStart?.Invoke(new ServerEventData(null, null, null, $"Https Server started on port {HttpsPort}"));
                }
                IsRunning = true;
                onServerStop += HaultServerThread;
                while (IsRunning)
                {
                    var context = listener.GetContext();
                    ContextRunner.RunWith(context);
                }
            }
            else
            {
                throw new ServerNotSupportedException($"HttpListener is not supported on {Environment.OSVersion.VersionString}");
            }
        }

        /// <summary>
        /// Automatically find all (GET|POST|DELETE|PUT|PATCH)Mapping controllers and register them as endpoints
        /// </summary>
        public static void RegisterEndpoints()
        {
            int controllerCount = 0;
            foreach (var controller in Assembly.GetCallingAssembly().GetTypes())
            {
                if (controller.GetCustomAttribute<RestController>() != null)
                {
                    RestController.ExtractRequestMethods(controller);
                    controllerCount++;
                }
            }
            if (controllerCount > 0)
            {
                var eventData = new ServerEventData
                {
                    exception = null,
                    message = $"Found {controllerCount} {(controllerCount > 1 ? "controllers" : "controller")}",
                    path = null,
                    status = null
                };
                onEndpointRegistrationFinished?.Invoke(eventData);
            }
        }

        public static void Stop()
        {
            if (serverThread.IsAlive)
            {
                onServerStop?.Invoke(new ServerEventData(null, null, null, "Server stopped"));
            }
            IsRunning = false;
        }
    }

#nullable enable
    public struct ServerEventData
    {
        public int? status;
        public string? path;
        public AbstractServerException? exception;
        public string? message;

        public ServerEventData(int? status, string? path, AbstractServerException? exception, string? message)
        {
            this.status = status;
            this.path = path;
            this.exception = exception;
            this.message = message;
        }
    }
#nullable disable
}
