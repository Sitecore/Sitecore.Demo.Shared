using System.Collections.Generic;

namespace Sitecore.Demo.Shared.Feature.Demo.Models
{
    public class Profile
  {
    public string Name { get; set; }
    public IEnumerable<PatternMatch> PatternMatches { get; set; }
  }
}