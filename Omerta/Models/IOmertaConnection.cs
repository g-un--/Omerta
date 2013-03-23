using System;
using System.Threading.Tasks;

namespace Omerta.Models
{
    public interface IOmertaConnection : IDisposable
    {
        Task Open();
        Task<long> Publish(string key, string value);
        IOmertaSubscriberConnection GetOpenSubscriberChannel();
        void Close(bool abort);
    }
}
