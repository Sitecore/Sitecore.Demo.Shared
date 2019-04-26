namespace Sitecore.Demo.Feature.Demo.Repositories
{
    using Sitecore.Analytics.Tracking;
    using Sitecore.Demo.Feature.Demo.Models;

    public interface IPageViewRepository
    {
        PageView Get(ICurrentPageContext pageContext);
    }
}