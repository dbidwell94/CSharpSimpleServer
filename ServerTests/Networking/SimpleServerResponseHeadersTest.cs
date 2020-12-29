using NUnit.Framework;
using SimpleServer;
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
        var rangeHeader = RangeHeader.Create(new RangeHeaderValue[] { new RangeHeaderValue(0, 50) });
        headers.SetRange(rangeHeader);
        Assert.AreEqual("bytes=0-50", headers.Range);

        rangeHeader = RangeHeader.Create(new RangeHeaderValue[] { new RangeHeaderValue(0, 50), new RangeHeaderValue(100, 500) });
        headers.SetRange(rangeHeader);
        Assert.AreEqual("bytes=0-50, 100-500", headers.Range);

        rangeHeader = RangeHeader.Create(new RangeHeaderValue[] { new RangeHeaderValue(0, 50), new RangeHeaderValue(100, null) });
        headers.SetRange(rangeHeader);
        Assert.AreEqual("bytes=0-50, 100-", headers.Range);

        rangeHeader = RangeHeader.Create(new RangeHeaderValue[] { new RangeHeaderValue(0, 50), new RangeHeaderValue(null, 500) });
        headers.SetRange(rangeHeader);
        Assert.AreEqual("bytes=0-50, -500", headers.Range);

        rangeHeader = RangeHeader.Create(new RangeHeaderValue[] { new RangeHeaderValue(0, 50), new RangeHeaderValue(null, null) });
        headers.SetRange(rangeHeader);
        Assert.AreEqual("bytes=0-50", headers.Range);

        rangeHeader = RangeHeader.Create(new RangeHeaderValue[] { new RangeHeaderValue(0, 50), new RangeHeaderValue() });
        headers.SetRange(rangeHeader);
        Assert.AreEqual("bytes=0-50", headers.Range);

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