using BookSleeve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omerta.Models
{
    public class OmertaChatConnection : IOmertaChatConnection
    {
        private dynamic connection;

        public OmertaChatConnection(dynamic connection)
        {
            this.connection = connection;
        }

        public Task Open()
        {
            return this.connection.Open();
        }

        public Task<long> Publish(string key, string value)
        {
            return this.connection.Publish(key, value);
        }

        public IOmertaSubscriberConnection GetOpenSubscriberChannel()
        {
            return this.connection.GetOpenSubscriberChannel();
        }

        public void Close(bool abort)
        {
            this.connection.Abort(abort);
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
                    this.connection.Dispose();
                }

                disposed = true;
            }
        }

        #endregion
    }
}
