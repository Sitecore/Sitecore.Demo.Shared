﻿using System.Web;
using Sitecore.Demo.Foundation.Dictionary.Repositories;
using Sitecore.Demo.Foundation.SitecoreExtensions.Extensions;
using Sitecore.Mvc.Helpers;

namespace Sitecore.Demo.Foundation.Dictionary.Extensions
{
    public static class SitecoreExtensions
    {
        public static string Dictionary(this SitecoreHelper helper, string relativePath, string defaultValue = "")
        {
            return DictionaryPhraseRepository.Current.Get(relativePath, defaultValue);
        }

        public static HtmlString DictionaryField(this SitecoreHelper helper, string relativePath, string defaultValue = "")
        {
            var item = DictionaryPhraseRepository.Current.GetItem(relativePath, defaultValue);
            if (item == null)
            {
                return new HtmlString(defaultValue);
            }

            return helper.Field(Templates.DictionaryEntry.Fields.Phrase, item);
        }
    }
}