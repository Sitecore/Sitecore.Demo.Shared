namespace Sitecore.Demo.Foundation.DependencyInjection.Infrastructure
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;

    public class MvcControllerServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvcControllers("Sitecore.Demo.Feature.*");
            serviceCollection.AddClassesWithServiceAttribute("Sitecore.Demo.Feature.*");
            serviceCollection.AddClassesWithServiceAttribute("Sitecore.Demo.Foundation.*");
        }
    }
}