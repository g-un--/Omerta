using BookSleeve;
using System;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Text;

namespace Omerta.Tests
{
    [TestFixture]
    public class RedisTests
    {
        [TestCase("localhost")]
        public void CanConnectToRedis(string host)
        {
            using (var connection = new RedisConnection(host: host))
            {
                connection.Open();
                var ms = connection.Wait(connection.Server.Ping());
                Assert.IsTrue(ms >= 0);
            }
        }

        [TestCase("localhost", "testChannel")]
        public void CanConnectToRedisAndChat(string host, string channelName)
        {
            using (var connection = new RedisConnection(host: host))
            {
                connection.Open();
                var subscriberChannel = connection.GetOpenSubscriberChannel();
                var messageSend = "test";
                var tcs = new TaskCompletionSource<string>();
             
                subscriberChannel.Subscribe(channelName, (channel, message) =>
                    {
                        var messageReceived = Encoding.UTF8.GetString(message);
                        tcs.SetResult(messageReceived);
                    });
                connection.Wait(connection.Publish(channelName, messageSend));
                tcs.Task.Wait(TimeSpan.FromMilliseconds(1000));
                Assert.IsTrue(tcs.Task.IsCompleted && tcs.Task.Result == messageSend);
            }
        }
    }
}
