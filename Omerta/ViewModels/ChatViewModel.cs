using Omerta.Models;
using ReactiveUI;
using ReactiveUI.Xaml;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monads;
using System.Collections.Immutable;

namespace Omerta.ViewModels
{
    public class ChatViewModel : ReactiveObject
    {
        public ReactiveAsyncCommand SendMessage { get; private set; }

        private string textMessage;
        public string TextMessage
        {
            get 
            {
                if (textMessage == null)
                    return string.Empty;
            
                return textMessage; 
            }
            set
            {
                textMessage = value;
                this.RaisePropertyChanged<ChatViewModel>("TextMessage");
            }
        }

        ObservableAsPropertyHelper<IList<string>> messages;
        public IList<string> Messages
        {
            get { return messages.Value; }
        }

        public ChatViewModel(string channelName, IChat chat)
        {
            this.SendMessage = new ReactiveAsyncCommand(null, 1 /*at a time*/);
            this.SendMessage.RegisterAsyncFunction(commandParam =>
                {
                    return chat.SendMessage(channelName, commandParam.ToString());
                })
                .Subscribe(_ =>
                {
                    this.TextMessage = string.Empty;
                });

            var receivedMessages = chat.ReceiveMessages(channelName)
                .Buffer(TimeSpan.FromMilliseconds(250), TaskPoolScheduler.Default)
                .Where(list => list.Count > 0)
                .Scan(ImmutableList.Create<string>(), (list, value) =>
                    {
                        if (value.Count > 0)
                        {
                            var builder = list.ToBuilder();

                            foreach (var item in value)
                                builder.Add(item);

                            return builder.ToImmutable();
                        }

                        return list;
                    });

            this.messages = new ObservableAsPropertyHelper<IList<string>>(
                receivedMessages,
                _ =>
                {
                    this.RaisePropertyChanged(viewModel => viewModel.Messages);
                });
         }
    }
}
