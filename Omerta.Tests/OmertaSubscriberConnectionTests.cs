using NUnit.Framework;
using Omerta.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omerta.Tests
{
    [TestFixture]
    public class OmertaSubscriberConnectionTests
    {
        [TestCase]
        public void SubscribeCallsSubscriberConnection()
        {
            dynamic subscriberConnection = new ExpandoObject();
            var methodCalled = false;
    
            subscriberConnection.Subscribe = new Action<string, Action<string, byte[]>>((key, handler) =>
            {
                methodCalled = true;
            });

            var omertaSubscriberConnection = new OmertaSubscriberConnection(subscriberConnection);
            omertaSubscriberConnection.Subscribe("test", null);

            Assert.IsTrue(methodCalled);
        }
    }
}
