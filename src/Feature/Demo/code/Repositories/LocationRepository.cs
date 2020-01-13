using System.Globalization;
using Sitecore.Analytics;
using Sitecore.Analytics.Tracking;
using Sitecore.Demo.Shared.Feature.Demo.Models;
using Sitecore.Demo.Shared.Foundation.DependencyInjection;

namespace Sitecore.Demo.Shared.Feature.Demo.Repositories
{
    [Service]
    public class LocationRepository
    {
        public Location GetCurrent()
        {
            return !Tracker.Current.Interaction.HasGeoIpData ? null : this.CreateLocation(Tracker.Current.Interaction.GeoData);
        }

        private Location CreateLocation(ContactLocation geoData)
        {
            if (geoData.Latitude == null || geoData.Longitude == null)
            {
                return null;
            }
            return new Location
            {
                BusinessName = geoData.BusinessName,
                Url = geoData.Url,
                City = geoData.City,
                Region = geoData.Region,
                Country = geoData.Country,
                Latitude = string.Format(CultureInfo.InvariantCulture, "{0:0.#######}", geoData.Latitude),
                Longitude = string.Format(CultureInfo.InvariantCulture, "{0:0.#######}", geoData.Longitude),
            };
        }
    }
}