namespace Sitecore.Demo.Feature.Demo.Repositories
{
    using System.Collections.Generic;
    using Sitecore.Demo.Feature.Demo.Models;

    public interface IEngagementPlanStateRepository
    {
        IEnumerable<EngagementPlanState> GetCurrent();
    }
}