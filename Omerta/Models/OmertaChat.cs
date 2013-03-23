using Autofac;
using BookSleeve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omerta.Models
{
    class OmertaChat : IChat, IDisposable
    {
        private IOmertaConnection connection;

        public OmertaChat(IOmertaConnection connection)
        {
            this.connection = connection;
        }

        public Task Open()
        {
            return connection.Open();
        }

        public Task SendMessage(string channel, string message)
        {
            return connection.Publish(channel, message);
        }

        public IObservable<string> ReceiveMessages(string channelName)
        {
            return Observable.Create<string>(observer =>
                {
                    var channel = connection.GetOpenSubscriberChannel();

                    channel.Subscribe(channelName, (channelNameReceived, messageReceived) =>
                        {
                            if (string.Compare(channelName, channelNameReceived, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                observer.OnNext(Encoding.UTF8.GetString(messageReceived));
                            }
                        });

                    return channel;
                });
        }

        #region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    connection.Close(true);
                    connection.Dispose();
                }

                disposed = true;

            }
        }

        #endregion
    }
}
