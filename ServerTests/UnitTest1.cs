using Xunit;
using SimpleServer.Networking.Data;

namespace ServerTests
{
    public class ResponseEntityTests
    {
        [Fact(DisplayName = "Create Response Entity With No Parameters")]
        public void NoParameters()
        {
            var entity = new ResponseEntity();
            Assert.Equal(null, entity.Data);
        }
    }
}
