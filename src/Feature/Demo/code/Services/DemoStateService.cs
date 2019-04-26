using System;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Demo.Feature.Demo.Services;
using Sitecore.Demo.Foundation.DependencyInjection;

namespace Sitecore.Demo.Feature.Demo.Services
{
    [Service(typeof(IDemoStateService))]
    public class DemoStateService : IDemoStateService
    {
        public DemoStateService(HttpContextBase httpContext)
        {
            this.HttpContext = httpContext;
        }

        public HttpContextBase HttpContext { get; }

        public bool IsDemoEnabled => !this.HttpContext?.Request?.Headers["X-DisableDemo"]?.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase) ?? Settings.GetBoolSetting("Demo.Enabled", true);
    }
}