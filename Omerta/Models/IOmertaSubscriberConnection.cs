using System;

namespace Omerta.Models
{
    public interface IOmertaSubscriberConnection : IDisposable
    {
        void Subscribe(string key, Action<string, byte[]> handler = null);
    }
}
