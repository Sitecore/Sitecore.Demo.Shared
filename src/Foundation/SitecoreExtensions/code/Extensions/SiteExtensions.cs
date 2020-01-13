﻿using System;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Sites;

namespace Sitecore.Demo.Shared.Foundation.SitecoreExtensions.Extensions
{
    public static class SiteExtensions
    {
        public static Item GetContextItem(this SiteContext site, ID derivedFromTemplateID)
        {
            if (site == null)
                throw new ArgumentNullException(nameof(site));

            var startItem = site.GetStartItem();
            return startItem?.GetAncestorOrSelfOfTemplate(derivedFromTemplateID);
        }

        public static Item GetRootItem(this SiteContext site)
        {
            if (site == null)
                throw new ArgumentNullException(nameof(site));

            return site.Database.GetItem(Context.Site.RootPath);
        }

        public static Item GetStartItem(this SiteContext site)
        {
            if (site == null)
                throw new ArgumentNullException(nameof(site));

            return site.Database.GetItem(Context.Site.StartPath);
        }

        public static Item GetSettingsItem(this SiteContext site)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            var defaultSettingsItem = site.GetRootItem().Children
                .FirstOrDefault(x => x.Name == "Settings");

            if (defaultSettingsItem == null)
            {
                throw new ArgumentNullException("Site settings not found", nameof(site));
            }

            return defaultSettingsItem;
        }
    }
}