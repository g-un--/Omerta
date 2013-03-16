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

            builder.RegisterType<RedisChat>()
                .As<IChat>();

            builder
                .Register<ChatViewModel>((context, parameters) => 
                    {
                        return new ChatViewModel("testChannel", context.Resolve<IChat>());
                    });

            builder.Register<ShellViewModel>((context, parameters) =>
                {
                    return new ShellViewModel(context.Resolve<ChatViewModel>());
                });
        }

    }
}