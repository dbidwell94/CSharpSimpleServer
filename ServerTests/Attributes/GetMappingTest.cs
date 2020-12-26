using System;
using Xunit;
using SimpleServer;
using SimpleServer.Attributes;
using SimpleServer.Networking.Data;
using SimpleServer.Networking;

[RestController("/test")]
public class ControllerTest
{
    [GetMapping("/test", Accepts = MediaTypes.ApplicationJson, Produces = MediaTypes.ApplicationJson)]
    public ResponseEntity TestGet()
    {
        return new ResponseEntity();
    }
}

public class GetMappingTest
{
    [Fact]
    public void TestServerPort()
    {
        Server.Start(8675);
        Server.RegisterEndpoints();
        Assert.True(Server.IsRunning);
        Assert.Equal(8675, Server.Port);
        Assert.False(Server.UsingHttps);
        Server.Stop();
        Assert.False(Server.IsRunning);
    }

    [Fact]
    public void TestMethodCache()
    {
        int methodMappingCount = 0;
        int methodEnumCount = 0;
        foreach (var map in AbstractMapping.Mapping)
        {
            methodMappingCount++;
        }
        foreach (var method in Enum.GetValues(typeof(HttpMethod)))
        {
            methodEnumCount++;
        }
        Assert.Equal(methodEnumCount, methodMappingCount);
        
    }
}