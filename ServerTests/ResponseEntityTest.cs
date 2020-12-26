using Xunit;
using SimpleServer.Networking.Data;
using System;
using System.Collections.Generic;

namespace ServerTests
{
    public class ResponseEntityTests
    {
        [Fact(DisplayName = "Create Response Entity With No Parameters")]
        public void NoParameters()
        {
            var entity = new ResponseEntity();
            Assert.True(Object.Equals(null, entity.Data));
            Assert.Equal(4, entity.GetDataAsBytes().Length);
        }

        [Fact(DisplayName = "Create Response Entity With Object As Parameters")]
        public void ObjectAsParameter()
        {
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments.Add("v", new int[] { 1, 2, 3, 4 });
            var entity = new ResponseEntity(arguments);
            Assert.Equal(15, entity.GetDataAsBytes().Length);
            Assert.Equal(entity.Data.GetType(), typeof(Dictionary<string, object>));
            Assert.Equal("{\"v\":[1,2,3,4]}", entity.JSON);
        }
    }
}
