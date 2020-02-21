using Sitecore.Sites;

namespace Sitecore.Demo.Shared.Foundation.Dictionary.Repositories
{
  public interface IDictionaryRepository
  {
    Models.Dictionary Get(SiteContext site);
  }
}