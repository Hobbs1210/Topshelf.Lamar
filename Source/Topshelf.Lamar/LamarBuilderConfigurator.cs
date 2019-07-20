using Lamar;
using System;
using System.Collections.Generic;
using Topshelf.Builders;
using Topshelf.Configurators;
using Topshelf.HostConfigurators;

namespace Topshelf.Lamar
{
    public class LamarBuilderConfigurator : HostBuilderConfigurator
    {
        private static IContainer _container;

        public LamarBuilderConfigurator(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container)); ;

        }

        public static IContainer Container => _container;

        public HostBuilder Configure(HostBuilder builder)
        {
            return builder;
        }

        public IEnumerable<ValidateResult> Validate()
        {
            yield break;
        }
    }
}
