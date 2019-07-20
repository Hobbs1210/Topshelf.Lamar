using System;
using Lamar;
using Topshelf.HostConfigurators;

namespace Topshelf.Lamar
{
    public static class HostConfiguratorExtensions
    {
        public static HostConfigurator UseLamar(this HostConfigurator configurator, IContainer container)
        {
            if (configurator == null) throw new ArgumentNullException(nameof(configurator));
            if (container == null) throw new ArgumentNullException(nameof(container));

            configurator.AddConfigurator(new LamarBuilderConfigurator(container));
            return configurator;
        }
    }
}
