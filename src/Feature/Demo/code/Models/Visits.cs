using System.Collections.Generic;

namespace Sitecore.Demo.Shared.Feature.Demo.Models
{
    public class Visits
  {
    public int EngagementValue { get; set; }
    public IEnumerable<PageView> PageViews { get; set; }
    public int TotalPageViews { get; set; }
    public int TotalVisits { get; set; }
    public IEnumerable<EngagementPlanState> EngagementPlanStates { get; set; }
  }
}