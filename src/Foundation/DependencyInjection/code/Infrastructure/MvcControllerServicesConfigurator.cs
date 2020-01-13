using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Sitecore.Demo.Shared.Foundation.DependencyInjection.Infrastructure
{
    public class MvcControllerServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvcControllers("Sitecore.Demo.Shared.Feature.*");
            serviceCollection.AddClassesWithServiceAttribute("Sitecore.Demo.Shared.Feature.*");
            serviceCollection.AddClassesWithServiceAttribute("Sitecore.Demo.Shared.Foundation.*");
        }
    }
}