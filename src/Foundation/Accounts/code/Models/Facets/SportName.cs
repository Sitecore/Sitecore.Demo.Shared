﻿using Sitecore.XConnect;
using System;

namespace Sitecore.Demo.Foundation.Accounts.Models.Facets
{
    [Serializable]
    public class SportName : Facet
    {
        public static string DefaultKey = "SportName";

        public string Value { get; set; }
    }
}