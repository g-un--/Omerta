using Omerta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omerta
{
    public class ShellViewModel : IShell 
    {
        public ChatViewModel ChatViewModel { get; private set; }

        public ShellViewModel(ChatViewModel chatViewModel)
        {
            this.ChatViewModel = chatViewModel;
        }
    }
}
