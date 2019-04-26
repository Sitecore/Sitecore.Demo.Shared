﻿using Sitecore.Data.Fields;
using Sitecore.Demo.Foundation.SitecoreExtensions.Models;

namespace Sitecore.Demo.Feature.Media.Models
{
    public class VideoViewModel : ItemBase
    {
        public string VideoEmbed { get; set; }

        public string VideoFile { get; set; }

        public string VideoFileSrc
        {
            get
            {
                if (string.IsNullOrEmpty(VideoFile)) return string.Empty;

                FileField fileField = Item.Fields[Templates.Video.Fields.VideoFile];
                if (fileField != null) return fileField.Src;

                return string.Empty;
            }
        }
    }
}