using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        private static CancellationTokenSource cancellationToken = new CancellationTokenSource();

        #region Properties
        public static bool IsRunning { get; private set; }
        public static bool UsingHttps { get; private set; }
        public static int Port { get; private set; }
        public static int HttpsPort { get; private set; }
        private static HttpListener listener;
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
            Run();
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
            Run();
            serverThread.Start();
        }

        private static void KeepThreadAlive()
        {
            while (true)
            {
                Thread.Sleep(500);
                if (cancellationToken.Token.IsCancellationRequested)
                {
                    IsRunning = false;
                    onServerStop?.Invoke(new ServerEventData(null, null, null, "Server stopped"));
                    cancellationToken = new CancellationTokenSource();
                    serverThread = new Thread(KeepThreadAlive);
                    if (listener.IsListening)
                    {
                        listener.Stop();
                    }
                }
            }
        }

        static Server()
        {
            serverThread = new Thread(KeepThreadAlive);
            ContextRunner.onRequestFinishedProcessing += (msg) => onRequestReceived?.Invoke(msg);
            ExceptionHandler.onError += (err) =>
            {
                var serverEvent = new ServerEventData(null, null, err, null);
                onServerError?.Invoke(serverEvent);
            };
            ServiceAttribute.RegisterServices();
        }

        private static async void Run()
        {

            if (HttpListener.IsSupported)
            {
                await Task.Run(() =>
                {
                    listener = new HttpListener();
                    listener.Start();
                    IsRunning = true;
                    listener.Prefixes.Add($"http://*:{Port}/");
                    onServerStart?.Invoke(new ServerEventData(null, null, null, $"Server started on port {Port}"));
                    if (UsingHttps)
                    {
                        listener.Prefixes.Add($"https://*:{HttpsPort}");
                        onServerStart?.Invoke(new ServerEventData(null, null, null, $"Https Server started on port {HttpsPort}"));
                    }
                    while (IsRunning)
                    {
                        try
                        {
                            var context = listener.GetContext();
                            ContextRunner.RunWith(context);
                        }
                        catch (HttpListenerException ex)
                        {
                            onServerError?.Invoke(new ServerEventData(null, null, null, ex.ToString()));
                        }
                    }
                }, cancellationToken.Token);
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
            cancellationToken.Cancel();
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
