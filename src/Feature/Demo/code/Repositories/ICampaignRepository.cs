using System.Collections.Generic;
using Sitecore.Demo.Shared.Feature.Demo.Models;

namespace Sitecore.Demo.Shared.Feature.Demo.Repositories
{
    public interface ICampaignRepository
  {
    Campaign GetCurrent();
    IEnumerable<Campaign> GetHistoric();
  }
}