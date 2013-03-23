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
using System.Reactive;
using Caliburn.Micro;

namespace Omerta.ViewModels
{
    public class ChatViewModel : Screen, IReactiveNotifyPropertyChanged
    {
        private readonly MakeObjectReactiveHelper reactiveHelper;
        private readonly ObservableAsPropertyHelper<IList<string>> messages;
        private readonly IChat chat;
        private string textMessage;

        public ReactiveAsyncCommand SendMessage { get; private set; }

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
                this.NotifyOfPropertyChange(() => this.TextMessage);
            }
        }

        
        public IList<string> Messages
        {
            get { return messages.Value; }
        }

        public ChatViewModel(string channelName, IChat chat)
        {
            reactiveHelper = new MakeObjectReactiveHelper(this);

            var openTask = chat.Open();
            var ticket = openTask;

            this.SendMessage = new ReactiveAsyncCommand(null, 1 /*at a time*/);
            this.SendMessage.RegisterAsyncFunction(commandParam =>
                {
                    var newTicket = SendMessageAsync(chat, openTask, channelName, commandParam as string).Unwrap();
                    ticket = newTicket;
                    return newTicket;
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
                    this.NotifyOfPropertyChange(() => this.Messages);
                });
         }

        private async Task<Task> SendMessageAsync(IChat chat, Task ticket, string channelName, string message)
        {
            await ticket;
            return chat.SendMessage(channelName, message);
        }

        public IObservable<IObservedChange<object, object>> Changed
        {
            get { return reactiveHelper.Changed; }
        }

        public IObservable<IObservedChange<object, object>> Changing
        {
            get { return reactiveHelper.Changing; }
        }

        public IDisposable SuppressChangeNotifications()
        {
            return reactiveHelper.SuppressChangeNotifications();
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
                chat.Dispose();

            base.OnDeactivate(close);
        }

        public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
    }
}
