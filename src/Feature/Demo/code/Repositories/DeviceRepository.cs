using Microsoft.Extensions.DependencyInjection;
using Sitecore.Analytics;
using Sitecore.CES.DeviceDetection;
using Sitecore.Demo.Shared.Feature.Demo.Models;
using Sitecore.Demo.Shared.Foundation.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Sitecore.Demo.Shared.Feature.Demo.Repositories
{
    [Service]
    public class DeviceRepository
    {
        private DeviceDetectionManagerBase deviceDetectionManager => ServiceLocator.ServiceProvider.GetRequiredService<DeviceDetectionManagerBase>();

        private Device current;

        public Device GetCurrent()
        {
            if (this.current != null)
            {
                return this.current;
            }

            if (!deviceDetectionManager.IsEnabled || !deviceDetectionManager.IsReady || string.IsNullOrEmpty(Tracker.Current.Interaction.UserAgent))
            {
                return null;
            }

            return this.current = this.CreateDevice(deviceDetectionManager.GetDeviceInformation(Tracker.Current.Interaction.UserAgent));
        }

        private Device CreateDevice(DeviceInformation deviceInformation)
        {
            return new Device
            {
                Title = string.Join(", ", deviceInformation.DeviceVendor, deviceInformation.DeviceModelName),
                Browser = deviceInformation.Browser
            };
        }
    }
}