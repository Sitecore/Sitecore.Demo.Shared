using System.Collections.Generic;
using Sitecore.Demo.Shared.Feature.Demo.Models;

namespace Sitecore.Demo.Shared.Feature.Demo.Repositories
{
    public interface IEngagementPlanStateRepository
    {
        IEnumerable<EngagementPlanState> GetCurrent();
    }
}