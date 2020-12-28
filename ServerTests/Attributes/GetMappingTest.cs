using System;
using NUnit.Framework;
using SimpleServer;
using SimpleServer.Attributes;
using SimpleServer.Networking.Data;
using SimpleServer.Networking;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.Http;
using SSMethod = SimpleServer.Networking.HttpMethod;

[RestController("/test")]
public class ControllerTest
{
    public static int TotalEndpoints { get; private set; } = 0;
    public const string TEST_GET_MESSAGE = "All Good!";

    static ControllerTest()
    {
        var controllerTest = Assembly.GetCallingAssembly().GetType("ControllerTest");
        foreach (var method in controllerTest.GetMethods())
        {
            if (method.GetCustomAttribute<AbstractMapping>() != null)
            {
                TotalEndpoints++;
            }
        }
    }

    [GetMapping("/test", Accepts = MediaTypes.ApplicationJson, Produces = MediaTypes.ApplicationJson)]
    public ResponseEntity TestGet()
    {
        return new ResponseEntity(TEST_GET_MESSAGE);
    }

    [GetMapping("/param/:id", Accepts = MediaTypes.ApplicationJson, Produces = MediaTypes.ApplicationJson)]
    public ResponseEntity ParametersTest([PathParam] long id)
    {
        return new ResponseEntity();
    }
}

[TestFixture]
public class GetMappingTest
{
    private const int TEST_PORT = 8675;
    [TearDown]
    public void TearDown()
    {
        foreach (SSMethod map in Enum.GetValues(typeof(SSMethod)))
        {
            AbstractMapping.Mapping[map] = new System.Collections.Generic.Dictionary<string, MappingInfo<AbstractMapping>>();
        }
    }


    [TestCase]
    [Order(1)]
    public void TestServerStartsAndStopsCorrectly()
    {
        AutoResetEvent serverStart = new AutoResetEvent(false);
        AutoResetEvent serverStop = new AutoResetEvent(false);
        Server.onServerStart += (data) => serverStart.Set();
        Server.onServerStop += (data) => serverStop.Set();

        Server.Start(TEST_PORT);
        if (!serverStart.WaitOne(3000, false))
        {
            Assert.Fail("Server did not send start message");
        }
        Assert.True(Server.IsRunning);
        Assert.AreEqual(TEST_PORT, Server.Port);
        Server.Stop();
        if (!serverStop.WaitOne(3000, false))
        {
            Assert.Fail("Server did not send stop event");
        }
        Assert.False(Server.IsRunning);
    }

    [TestCase]
    [Order(2)]
    public void TestServerMethodsAreGeneratedCorrectly()
    {
        int httpMethodCount = 0;
        int serverMethodMapCount = 0;
        foreach (var method in Enum.GetValues(typeof(SSMethod)))
        {
            httpMethodCount++;
        }
        foreach (var map in AbstractMapping.Mapping)
        {
            serverMethodMapCount++;
        }
        Assert.AreEqual(httpMethodCount, serverMethodMapCount);
    }

    [TestCase]
    [Order(3)]
    public void TestServerMappingsRetreviedCorrectly()

    {
        Server.RegisterEndpoints();
        int registeredEndpoints = 0;
        MappingInfo<AbstractMapping>? testEndpointData = null;
        foreach (var map in AbstractMapping.Mapping)
        {
            foreach (var endpoint in map.Value)
            {
                registeredEndpoints++;
                testEndpointData = endpoint.Value;
            }
        }
        Assert.AreEqual(ControllerTest.TotalEndpoints, registeredEndpoints);
        if (testEndpointData == null)
        {
            Assert.Fail("Did not find the test mapping to assert");
        }
    }

    [TestCase]
    [Order(4)]
    public async Task TestNoParametersGETWorksCorrectly()
    {
        ManualResetEvent serverStart = new ManualResetEvent(false);
        ManualResetEvent serverStop = new ManualResetEvent(false);
        Server.onServerStart += (data) => serverStart.Set();
        Server.onServerStop += (data) => serverStop.Set();
        Server.RegisterEndpoints();
        Server.Start(TEST_PORT);
        while (!serverStart.WaitOne(3000, false))
        {
            Assert.Fail("Server did not send start message");
        }
        MappingInfo<AbstractMapping> testMapping = AbstractMapping.FindPath("/test", SSMethod.GET, null);
        Assert.AreEqual(0, testMapping.RequiredParams.Count);
        Assert.AreEqual(null, testMapping.RequiredRequestBody);
        HttpClient client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:{TEST_PORT}/test");
        Assert.AreEqual(200, (int)response.StatusCode);
        var stringResponse = await response.Content.ReadAsStringAsync();
        Assert.AreEqual($"\"{ControllerTest.TEST_GET_MESSAGE}\"", stringResponse);
        Assert.AreEqual(MediaTypes.ApplicationJson, response.Content.Headers.ContentType.ToString());
        var byteLength = new ResponseEntity(ControllerTest.TEST_GET_MESSAGE).GetDataAsBytes().Length;
        Assert.AreEqual(byteLength, response.Content.Headers.ContentLength);
        Server.Stop();
        if (!serverStop.WaitOne(3000, false))
        {
            Assert.Fail("Server did not fire stop event");
        }
    }
}