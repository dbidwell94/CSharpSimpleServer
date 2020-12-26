using System;
using NUnit.Framework;
using NUnit.Mocks;
using SimpleServer;
using SimpleServer.Attributes;
using SimpleServer.Networking.Data;
using SimpleServer.Networking;
using System.Threading;

[RestController("/test")]
public class ControllerTest
{
    [GetMapping("/test", Accepts = MediaTypes.ApplicationJson, Produces = MediaTypes.ApplicationJson)]
    public ResponseEntity TestGet()
    {
        return new ResponseEntity();
    }
}

[TestFixture]
public class GetMappingTest
{
    [TearDown]
    public void TearDown()
    {
        foreach (HttpMethod map in Enum.GetValues(typeof(HttpMethod)))
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

        Server.Start(8675);
        if (!serverStart.WaitOne(3000, false))
        {
            Assert.Fail("Server did not send start message");
        }
        Assert.True(Server.IsRunning);
        Assert.AreEqual(8675, Server.Port);
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
        foreach (var method in Enum.GetValues(typeof(HttpMethod)))
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
        Assert.AreEqual(1, registeredEndpoints);
        if (testEndpointData == null)
        {
            Assert.Fail("Did not find the test mapping to assert");
        }
        Assert.AreEqual(0, testEndpointData.Value.RequiredParams.Count);
        Assert.False(testEndpointData.Value.RequiredRequestBody.HasValue);
        Assert.AreEqual("/test", testEndpointData.Value.Mapping.Path);
        Assert.AreEqual(MediaTypes.ApplicationJson, testEndpointData.Value.Mapping.Produces);
        Assert.AreEqual(MediaTypes.ApplicationJson, testEndpointData.Value.Mapping.Accepts);
    }
}