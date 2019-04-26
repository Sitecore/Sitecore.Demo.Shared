﻿using Sitecore.Demo.Foundation.SitecoreExtensions.Models;

namespace Sitecore.Demo.Feature.Components.Models
{
    public class Metadata : ItemBase
    {
        public string MetadataTitle { get; set; }

        public string MetadataDescription { get; set; }

        public string MetadataKeywords { get; set; }
    }
}