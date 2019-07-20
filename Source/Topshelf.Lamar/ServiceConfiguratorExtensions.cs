using Topshelf.ServiceConfigurators;

namespace Topshelf.Lamar
{
    public static class ServiceConfiguratorExtensions
    {
        public static ServiceConfigurator<T> ConstructUsingLamar<T>(this ServiceConfigurator<T> configurator) where T : class
        {
            configurator.ConstructUsing(serviceFactory => LamarBuilderConfigurator.Container.GetInstance<T>());
            return configurator;
        }

    }
}
