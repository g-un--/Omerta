using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omerta.Models
{
    public class OmertaSubscriberConnection : IOmertaSubscriberConnection
    {
        private dynamic subscriberConnection;

        public OmertaSubscriberConnection(dynamic subscriberConnection)
        {
            this.subscriberConnection = subscriberConnection;
        }

        public void Subscribe(string key, Action<string, byte[]> handler = null)
        {
            this.subscriberConnection.Subscribe(key, handler);
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
                    this.subscriberConnection.Dispose();
                }

                disposed = true;
            }
        }

        #endregion
    }
}
