﻿using Sitecore.XConnect;
using System;

namespace Sitecore.Demo.Foundation.Accounts.Models.Facets
{
    [Serializable]
    public class SportType : Facet
    {
        public static string DefaultKey = "SportType";

        public string Value { get; set; }
    }
}