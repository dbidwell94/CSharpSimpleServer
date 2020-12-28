# CSharpSimpleServer
A simple HTTP Server for C# .NET Core. Very much a work in progress, and modeled closely after Java Spring backend. Fires up quickly and is very responsive. Not for enterprise applications, but for client side server applications (I.E. Plex)

## Usage
The basic usage of this program is as such:

```csharp
using System;
using SimpleServer;
using System.Threading;

namespace TestImplementation
{
    static class Program
    {
        static void Main(string[] args)
        {
            // This will find all RestControllers
            Server.RegisterEndpoints();
            // This will start the server on a seperate thread on port 2019
            Server.Start(2019);
            // Below are the current server events that you can subscribe to.
            Server.onRequestReceived += LogMessage;
            Server.onServerStart += LogMessage;
            Server.onServerStop += LogMessage;
            Server.onEndpointRegistrationFinished += LogMessage;
            Server.onServerError += LogMessage;
        }

        private static void LogMessage(ServerEventData eventData)
        {
            string message = "";
            if (eventData.exception != null)
            {
                Console.WriteLine(eventData.exception.StackTrace);
                Console.WriteLine(eventData.exception.Message);
                Console.WriteLine(eventData.exception.TargetSite);
            }
            if (eventData.path != null)
            {
                message += $"{eventData.path.ToString()}";
            }
            if (eventData.status != null)
            {
                message += $" {eventData.status.ToString()}";
            }
            if (eventData.message != null)
            {
                message += $" -- {eventData.message}";
            }
            Console.WriteLine(message);
        }
    }
}

```

## So.. how do I define my endpoints?
That's easy: like this...

```csharp
using System;
using System.Collections.Generic;
using SimpleServer.Attributes;
using SimpleServer.Networking.Data;

namespace TestImplementation.Controllers
{
    // Currently, the path here does nothing. [RestController] just tells SimpleServer that this class contains endpoints
    [RestController("/")]
    class TestController
    {
        [GetMapping("/", Produces = MediaTypes.ApplicationJson, Accepts = MediaTypes.ApplicationJson)]
        public ResponseEntity TestMessage()
        {
            return new ResponseEntity("test");
        }

        // Notice the :videoName in the path? That is how you define a path parameter
        [GetMapping("/video/:videoName", Produces = MediaTypes.ApplicationJson, Accepts = MediaTypes.ApplicationJson)]
        // Path parameters must be defined marked with a [PathParam] attribute and named exactly how you named it in the path
        public ResponseEntity PathParameterTest([PathParam] string videoName)
        {
            Dictionary<string, object> toReturn = new Dictionary<string, object>();
            toReturn.Add("videoName", videoName);
            return new ResponseEntity(toReturn);
        }
    }
}
```

## How does it work?

Responses in SimpleServer are handled with the ResponseEntity class. The ResponseEntity will serialize the data as JSON and send it
as bytes to whatever requested the data. The JSON Serializer that SimpleServer uses is from [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
