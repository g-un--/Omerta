using BookSleeve;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Omerta.Tests
{
    [TestClass]
    public class RedisTests
    {
        [TestMethod]
        public void CanConnectToRedis()
        {
            using (var connection = new RedisConnection("localhost"))
            {
                connection.Open();
                var ms = connection.Wait(connection.Server.Ping());
                Assert.IsTrue(ms >= 0);
            }
        }
    }
}
