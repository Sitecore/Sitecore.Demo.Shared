using Sitecore.Analytics.Tracking;
using Sitecore.Demo.Shared.Feature.Demo.Models;

namespace Sitecore.Demo.Shared.Feature.Demo.Repositories
{
    public interface IPageViewRepository
    {
        PageView Get(ICurrentPageContext pageContext);
    }
}