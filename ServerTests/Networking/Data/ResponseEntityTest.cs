using NUnit.Framework;
using SimpleServer.Networking.Data;
using System;
using System.Collections.Generic;

namespace ServerTests
{
    [TestFixture]
    public class ResponseEntityTests
    {
        [TestCase]
        public void NoParameters()
        {
            var entity = new ResponseEntity();
            Assert.True(Object.Equals(null, entity.Data));
            Assert.AreEqual(4, entity.GetDataAsBytes().Length);
        }

        [TestCase]
        public void ObjectAsParameter()
        {
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments.Add("v", new int[] { 1, 2, 3, 4 });
            var entity = new ResponseEntity(arguments);
            Assert.AreEqual(15, entity.GetDataAsBytes().Length);
            Assert.AreEqual(typeof(Dictionary<string, object>), entity.Data.GetType());
            Assert.AreEqual("{\"v\":[1,2,3,4]}", entity.JSON);
        }
    }
}
