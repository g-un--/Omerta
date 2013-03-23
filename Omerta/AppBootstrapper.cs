using Caliburn.Micro.Autofac;

namespace Omerta
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Autofac;
    using System.ComponentModel;
    using Omerta.Models;
    using Omerta.ViewModels;
    using BookSleeve;
    using System.Dynamic;
    using System.Threading.Tasks;

    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        protected override void ConfigureBootstrapper()
        {
            //  you must call the base version first!
            base.ConfigureBootstrapper();
            //  override namespace naming convention
            EnforceNamespaceConvention = false;
            //  change our view model base type
            ViewModelBaseType = typeof(IShell);
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            builder
                .Register<RedisConnection>((context, parameters) =>
                 {
                    return new RedisConnection(parameters.Named<string>("host"));
                 }).InstancePerLifetimeScope();

            builder
                .Register<IOmertaSubscriberConnection>((context, parameters) =>
                 {
                     var connection = context.Resolve<RedisConnection>();
                     var subscriberChannel = connection.GetOpenSubscriberChannel();

                     dynamic redisSubscriberConnection = new ExpandoObject();
                     redisSubscriberConnection.Subscribe = new Action<string, Action<string, byte[]>>((key, handler) =>
                     {
                         subscriberChannel.Subscribe(key, handler);
                     });

                     return new OmertaSubscriberConnection(redisSubscriberConnection);
                 });

            builder
                .Register<IOmertaConnection>((context, parameters) =>
                 {
                    var connection = context.Resolve<RedisConnection>(parameters);

                    dynamic redisConnection = new ExpandoObject();
                    redisConnection.Open = new Func<Task>(() =>
                    {
                        return connection.Open();
                    });

                    redisConnection.Publish = new Func<string, string, Task<long>>((key, value) =>
                    {
                        return connection.Publish(key, value);
                    });

                    redisConnection.GetOpenSubscriberChannel = new Func<IOmertaSubscriberConnection>(() =>
                    {
                        return context.Resolve<IOmertaSubscriberConnection>();
                    });

                    redisConnection.Close = new Action<bool>((abort) =>
                    {
                        connection.Close(abort);
                    });

                    return new OmertaChatConnection(redisConnection);
                });

            builder
                .Register<OmertaChat>((context, parameters) =>
                 {
                     return new OmertaChat(
                         context.Resolve<IOmertaConnection>(parameters));
                 })
                .As<IChat>();

            builder
                .Register<ChatViewModel>((context, parameters) => 
                    {
                        return new ChatViewModel(parameters.Named<string>("channelName"), context.Resolve<IChat>(parameters));
                    });

            builder
                .Register<ShellViewModel>((context, parameters) =>
                {
                    //TODO remove this 
                    NamedParameter channelName = new NamedParameter("channelName", "testChannel");
                    NamedParameter host = new NamedParameter("host", "localhost");
                    return new ShellViewModel(context.Resolve<ChatViewModel>(channelName, host));
                });
        }

    }
}