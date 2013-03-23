using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omerta.Models
{
    public interface IChat : IDisposable
    {
        Task Open();
        Task SendMessage(string channelName, string message);
        IObservable<string> ReceiveMessages(string channelName); 
    }
}
