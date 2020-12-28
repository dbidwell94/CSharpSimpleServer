using System.Collections.Generic;
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

    [GetMapping("/param/:myId", Accepts = MediaTypes.ApplicationJson, Produces = MediaTypes.ApplicationJson)]
    public ResponseEntity ParametersTest([PathParam] long myId)
    {
        var response = new Dictionary<string, object>();
        response.Add("id", myId);
        return new ResponseEntity(response);
    }
}

[TestFixture]
public class GetMappingTest
{
    AutoResetEvent serverStart;
    AutoResetEvent serverStop;
    private const int TEST_PORT = 8675;

    #region Helper Methods
    private void ServerStartEvent(ServerEventData data)
    {
        serverStart.Set();
    }

    private void ServerStopEvent(ServerEventData data)
    {
        serverStop.Set();
    }

    private async Task WaitForServerStart(int timeout = 3000)
    {
        await Task.Run(() =>
        {
            while (!serverStart.WaitOne(timeout, false))
            {
                Assert.Fail("Server did not send start event");
            }
        });
    }

    private async Task WaitForServerStop(int timeout = 3000)
    {
        await Task.Run(() =>
        {
            while (!serverStop.WaitOne(timeout, false))
            {
                Assert.Fail("Server did not send stop event");
            }
        });
    }
    #endregion

    [TearDown]
    public void TearDown()
    {
        foreach (SSMethod map in Enum.GetValues(typeof(SSMethod)))
        {
            AbstractMapping.Mapping[map] = new System.Collections.Generic.Dictionary<string, MappingInfo<AbstractMapping>>();
        }
        Server.onServerStart -= ServerStartEvent;
        Server.onServerStop -= ServerStopEvent;
    }

    [SetUp]
    public void StartUp()
    {
        serverStart = new AutoResetEvent(false);
        serverStop = new AutoResetEvent(false);
        Server.onServerStart += ServerStartEvent;
        Server.onServerStop += ServerStopEvent;
    }

    [TestCase]
    [Order(1)]
    public async Task TestServerStartsAndStopsCorrectly()
    {
        Server.Start(TEST_PORT);
        await WaitForServerStart();
        Assert.True(Server.IsRunning);
        Assert.AreEqual(TEST_PORT, Server.Port);
        Server.Stop();
        await WaitForServerStop();
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
        Server.RegisterEndpoints();
        Server.Start(TEST_PORT);
        await WaitForServerStart();
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
        await WaitForServerStop();
    }

    [TestCase]
    [Order(5)]
    public async Task TestParamsGETWorksCorrectly()
    {
        Server.RegisterEndpoints();
        Server.Start(TEST_PORT);
        await WaitForServerStart();
        var client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:{TEST_PORT}/param/5");
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(responseString, "{\"id\":5}");
        Server.Stop();
        await WaitForServerStop();
    }
}