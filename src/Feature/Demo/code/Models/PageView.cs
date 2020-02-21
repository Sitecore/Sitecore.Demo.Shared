using System;

namespace Sitecore.Demo.Shared.Feature.Demo.Models
{
    public class PageView
  {
    public string FullPath { get; set; }
    public string Path { get; set; }
    public TimeSpan Duration { get; set; }
    public bool HasEngagementValue { get; set; }
    public bool HasMvTest { get; set; }
    public bool HasPersonalisation { get; set; }
  }
}