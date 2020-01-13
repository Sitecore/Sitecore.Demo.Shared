using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.Demo.Shared.Feature.Accounts.Services
{
    public interface IProfileSettingsService
    {
        IEnumerable<string> GetInterests();
        Item GetUserDefaultProfile();
    }
}