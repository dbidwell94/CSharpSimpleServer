using NUnit.Framework;
using SimpleServer.Networking;
using SimpleServer.Networking.Headers;

[TestFixture]
class HeaderTests
{
    [TestCase]
    [Order(0)]
    public void CheckHeaderRangePopulatedCorrectly()
    {
        var headers = new ServerResponseHeaders();
        headers.SetRange(RangeHeader.Create(new RangeHeaderValue(0, 50)));
        Assert.AreEqual("bytes 0-50/50", headers.Range);

        headers.SetRange(RangeHeader.Create(new RangeHeaderValue()));
        Assert.AreEqual("bytes 0-0/0", headers.Range);

        headers = new ServerResponseHeaders();
        Assert.AreEqual(null, headers.Range);
    }

    [TestCase]
    [Order(1)]
    public void CheckCorsPopulatedCorrectly()
    {
        var headers = new ServerResponseHeaders();
        headers.SetCors(CorsHeader.BuildHeader("*"));
        Assert.AreEqual("*", headers.Cors);

        headers.SetCors(CorsHeader.BuildHeader("http://localhost:2019/test"));
        Assert.AreEqual("localhost:2019", headers.Cors);

        headers.SetCors(CorsHeader.BuildHeader("http://www.google.com"));
        Assert.AreEqual("www.google.com", headers.Cors);
    }
}
