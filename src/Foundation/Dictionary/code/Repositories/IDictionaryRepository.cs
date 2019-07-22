using Sitecore.Demo.Foundation.Dictionary.Models;
using Sitecore.Sites;

namespace Sitecore.Demo.Foundation.Dictionary.Repositories
{
  public interface IDictionaryRepository
  {
    Models.Dictionary Get(SiteContext site);
  }
}